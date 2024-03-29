﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProKnow.Exceptions;
using ProKnow.Patient;
using ProKnow.Upload;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    /// <summary>
    /// Test utilities
    /// </summary>
    [TestClass]
    public static class TestHelper
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

        [AssemblyCleanup()]
        public static async Task AssemblyCleanup()
        {
            // Delete any extraneous SDK test workspaces
            var workspaces = await _proKnow.Workspaces.QueryAsync();
            foreach (var workspace in workspaces)
            {
                // Make sure workspace name starts with "SDK-" and contains "Test" to protect from accidental deletion of workspaces
                if (workspace.Name.StartsWith("SDK-") && workspace.Name.Contains("Test"))
                {
                    // Turn off protection, if necessary
                    if (workspace.Protected)
                    {
                        workspace.Protected = false;
                        await workspace.SaveAsync();
                    }
                    await _proKnow.Workspaces.DeleteAsync(workspace.Id);
                }
            }
        }

        /// <summary>
        /// Creates a patient asynchronously in workspace {testClassName} with MRN {testNumber}-Mrn and name {testNumber}-Name
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <param name="testNumber">The test number</param>
        /// <param name="testData">The optional test data subdirectory or file to upload</param>
        /// <param name="birthDate">The optional birthdate in the format "YYYY-MM-DD"</param>
        /// <param name="sex">The optional sex, one of "M", "F", or "O"</param>
        /// <param name="metadata">The optional metadata (custom metrics) names and values</param>
        /// <returns></returns>
        public static async Task<PatientItem> CreatePatientAsync(string testClassName, int testNumber, string testData = null,
            string birthDate = null, string sex = null, IDictionary<string, object> metadata = null)
        {
            // Find the workspace for this test
            var workspaceName = $"SDK-{testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.ResolveByNameAsync(workspaceName);

            // Create the patient
            var mrn = $"{testNumber}-Mrn";
            var name = $"{testNumber}-Name";
            var patientItem = await _proKnow.Patients.CreateAsync(workspaceItem.Id, mrn, name, birthDate, sex);

            // Upload test files, if requested
            if (testData != null)
            {
                var uploadPath = Path.Combine(TestSettings.TestDataRootDirectory, testData);
                var overrides = new UploadFileOverrides
                {
                    Patient = new PatientOverridesSchema { Mrn = mrn, Name = name }
                };
                var uploadResults = await _proKnow.Uploads.UploadAsync(workspaceItem, uploadPath, overrides);
                await _proKnow.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);

                // Refresh patient
                await patientItem.RefreshAsync();
            }

            // Add custom metric values
            if (metadata != null)
            {
                await patientItem.SetMetadataAsync(metadata);
                await patientItem.SaveAsync();
                await patientItem.RefreshAsync();
            }

            return patientItem;
        }

        /// <summary>
        /// Creates an array of patients asynchronously in workspace {testClassName} with MRN {testNumber}-i-Mrn and name {testNumber}-i-Name
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <param name="testNumber">The test number</param>
        /// <param name="numPatients">The number of patients to create (informs the range of values of "i" for MRN and name)</param>
        /// <returns></returns>
        public static async Task<IList<PatientItem>> CreateMultiPatientAsync(string testClassName, int testNumber, int numPatients)
        {
            // Find the workspace for this test
            var workspaceName = $"SDK-{testClassName}-{testNumber}";
            var workspaceItem = await _proKnow.Workspaces.ResolveByNameAsync(workspaceName);

            // Limit number of active requests
            var throttler = new SemaphoreSlim(500);
            var patientCreateTasksResults = new ConcurrentBag<PatientItem>();
            var finalTaskList = new List<Task>();

            async Task PatientCreate(string mrn, string name, string workspaceId)
            {
                try
                {
                    patientCreateTasksResults.Add(await _proKnow.Patients.CreateAsync(workspaceId, mrn, name));
                }
                finally
                {
                    throttler.Release();
                }
            }

            for (int i = 0; i < numPatients; i++)
            {
                var mrn = $"{testNumber}-{i}-Mrn";
                var name = $"{testNumber}-{i}-Name";

                // Create the patients when allowed
                throttler.Wait();
                finalTaskList.Add(PatientCreate(mrn, name, workspaceItem.Id));
            }
            await Task.WhenAll(finalTaskList);

            return patientCreateTasksResults.ToList();
        }

        /// <summary>
        /// Creates a test workspace asynchronously with name SDK-{testClassName}-{testNumber}
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <param name="testNumber">The test number</param>
        /// <returns>The created workspace item</returns>
        public static async Task<WorkspaceItem> CreateWorkspaceAsync(string testClassName, int testNumber)
        {
            var workspaceName = $"SDK-{testClassName}-{testNumber}";
            return await _proKnow.Workspaces.CreateAsync(workspaceName.ToLower(), workspaceName, false);
        }

        /// <summary>
        /// Deletes test collections asynchronously
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        public static async Task DeleteCollectionsAsync(string testClassName)
        {
            // Look for both workspace and organization collections
            foreach (var workspaceNameOrNull in new string[] { testClassName, null })
            {
                try
                {
                    var collectionSummaries = await _proKnow.Collections.QueryAsync(workspaceNameOrNull);
                    foreach (var collectionSummary in collectionSummaries)
                    {
                        if (collectionSummary.Name.Contains(testClassName))
                        {
                            await _proKnow.Collections.DeleteAsync(collectionSummary.Id);
                        }
                    }
                }
                catch
                {
                    // Ignore exception that occurs when workspace does not exist
                }
            }
        }

        /// <summary>
        /// Deletes all of the custom metrics for a test
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        public static async Task DeleteCustomMetricsAsync(string testClassName)
        {
            var customMetrics = await _proKnow.CustomMetrics.QueryAsync();
            foreach (var customMetric in customMetrics)
            {
                if (customMetric.Name.Contains(testClassName))
                {
                    await _proKnow.CustomMetrics.DeleteAsync(customMetric.Id);
                }
            }
        }

        /// <summary>
        /// Deletes the scorecard templates for a test
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        public static async Task DeleteScorecardTemplatesAsync(string testClassName)
        {
            var scorecardTemplates = await _proKnow.ScorecardTemplates.QueryAsync();
            foreach (var scorecardTemplate in scorecardTemplates)
            {
                if (scorecardTemplate.Name.Contains(testClassName))
                {
                    await _proKnow.ScorecardTemplates.DeleteAsync(scorecardTemplate.Id);
                }
            }
        }

        /// <summary>
        /// Deletes all workspaces for a test asynchronously
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        public static async Task DeleteWorkspacesAsync(string testClassName)
        {
            var workspaces = await _proKnow.Workspaces.QueryAsync();
            foreach (var workspace in workspaces)
            {
                // Make sure workspace name starts with "SDK-" and contains test class name to protect from accidental deletion of workspaces
                if (workspace.Name.StartsWith("SDK-") && workspace.Name.Contains(testClassName))
                {
                    // Turn off protection, if necessary
                    if (workspace.Protected)
                    {
                        workspace.Protected = false;
                        await workspace.SaveAsync();
                    }
                    await _proKnow.Workspaces.DeleteAsync(workspace.Id);
                }
            }
        }

        /// <summary>
        /// Determines whether two files have the same content
        /// </summary>
        /// <param name="path1">The full path to the first file</param>
        /// <param name="path2">The full path to the second file</param>
        /// <returns>True if the two files have the same content, otherwise false</returns>
        public static bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        Console.WriteLine($"Files are different at position {i}.");
                        return false;
                    }
                }
                return true;
            }
            Console.WriteLine($"Files have different lengths {file1.Length} bytes vs. {file2.Length} bytes.");
            return false;
        }
    }
}
