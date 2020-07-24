using ProKnow.Patient;
using ProKnow.Upload;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    /// <summary>
    /// Test utilities
    /// </summary>
    public static class TestHelper
    {
        private static readonly ProKnowApi _proKnow = TestSettings.ProKnow;

        /// <summary>
        /// Creates a test patient asynchronously
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <returns>The summary of the created patient</returns>
        public static async Task<PatientSummary> CreatePatientAsync(string testClassName)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(testClassName);
            await _proKnow.Patients.CreateAsync(workspaceItem.Id, testClassName, testClassName);
            return await _proKnow.Patients.FindAsync(workspaceItem.Id, t => t.Name == testClassName);
        }

        /// <summary>
        /// Creates a patient asynchronously in workspace {testClassName} with MRN {testNumber}-Mrn and name {testNumber}-Name
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <param name="testNumber">The test number</param>
        /// <param name="testData">The optional test data subdirectory or file to upload</param>
        /// <param name="numberOfEntitiesAndSros">The expected number of entities and SROs uploaded if test data was specified</param>
        /// <param name="birthDate">The optional birthdate in the format "YYYY-MM-DD"</param>
        /// <param name="sex">The optional sex, one of "M", "F", or "O"</param>
        /// <param name="metadata">The optional metadata (custom metrics) names and values</param>
        /// <returns></returns>
        public static async Task<PatientItem> CreatePatientAsync(string testClassName, int testNumber, 
            string testData = null, int numberOfEntitiesAndSros = 0, string birthDate = null, string sex = null,
            IDictionary<string, object> metadata = null)
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
                    Patient = new PatientCreateSchema { Mrn = mrn, Name = name }
                };
                var uploadBatch = await _proKnow.Uploads.UploadAsync(workspaceItem.Id, uploadPath, overrides);
                var uploadedEntityIds = uploadBatch.Patients.SelectMany(p => p.Entities.Select(e => e.Id));
                var uploadedSroIds = uploadBatch.Patients.SelectMany(p => p.Sros.Select(s => s.Id));

                // Wait until uploaded test files have processed
                while (true)
                {
                    await patientItem.RefreshAsync();
                    var entitySummaries = patientItem.FindEntities(e => uploadedEntityIds.Contains(e.Id));
                    var sroSummaries = patientItem.Studies.SelectMany(s => s.Sros.Where(r => uploadedSroIds.Contains(r.Id))).ToList();
                    if (entitySummaries.Count() + sroSummaries.Count() == numberOfEntitiesAndSros)
                    {
                        var statuses = entitySummaries.Select(e => e.Status).Union(sroSummaries.Select(s => s.Status)).Distinct().ToList();
                        if (statuses.Count == 1 && statuses[0] == "completed")
                        {
                            break;
                        }
                    }
                }
            }

            // Add custom metric values
            if (metadata != null)
            {
                await patientItem.SetMetadataAsync(metadata);
                await patientItem.SaveAsync();
            }

            return patientItem;
        }

        /// <summary>
        /// Creates a test workspace asynchronously
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <returns>The created workspace item</returns>
        public static async Task<WorkspaceItem> CreateWorkspaceAsync(string testClassName)
        {
            return await _proKnow.Workspaces.CreateAsync(testClassName.ToLower(), testClassName, false);
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
        /// Deletes a test custom metric asynchronously
        /// </summary>
        /// <param name="customMetric">The ProKnow ID or name of the custom metric</param>
        public static async Task DeleteCustomMetricAsync(string customMetric)
        {
            // If the custom metric exists
            var customMetricItem = await _proKnow.CustomMetrics.ResolveAsync(customMetric);
            if (customMetricItem != null)
            {
                // Request the deletion
                await _proKnow.CustomMetrics.DeleteAsync(customMetricItem.Id);
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
        /// Deletes a test workspace asynchronously
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        public static async Task DeleteWorkspaceAsync(string testClassName)
        {
            // If the workspace exists
            IList<WorkspaceItem> workspaces = await _proKnow.Workspaces.QueryAsync();
            var workspaceItem = workspaces.FirstOrDefault(w => w.Name == testClassName);
            if (workspaceItem != null)
            {
                // Request the deletion
                await _proKnow.Workspaces.DeleteAsync(workspaceItem.Id);
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
                // Include "SDK" to protect from accidental deletion of workspaces
                if (workspace.Name.Contains("SDK") && workspace.Name.Contains(testClassName))
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
