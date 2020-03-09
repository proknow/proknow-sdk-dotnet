using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProKnow.Patient;

namespace ProKnow.Test
{
    /// <summary>
    /// Test utilities
    /// </summary>
    public static class TestHelper
    {
        private static ProKnow _proKnow = TestSettings.ProKnow;

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
        /// Deletes the custom metrics for a test
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
        /// Creates a test workspace asynchronously
        /// </summary>
        /// <param name="testClassName">The test class name</param>
        /// <returns>The created workspace item</returns>
        public static async Task<WorkspaceItem> CreateWorkspaceAsync(string testClassName)
        {
            return await _proKnow.Workspaces.CreateAsync(testClassName.ToLower(), testClassName, false);
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
        /// Deletes a patient asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="mrn">The patient medical record number (MRN) or ID</param>
        public static async Task DeletePatientAsync(string workspaceId, string mrn)
        {
            var patientSummary = await _proKnow.Patients.FindAsync(workspaceId, t => t.Mrn == mrn);
            if (patientSummary != null)
            {
                await _proKnow.Patients.DeleteAsync(workspaceId, patientSummary.Id);
                while (await _proKnow.Patients.FindAsync(workspaceId, p => p.Mrn == mrn) != null)
                {
                    Thread.Sleep(50);
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
