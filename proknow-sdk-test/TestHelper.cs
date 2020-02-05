using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProKnow.Test
{
    /// <summary>
    /// Test utilities
    /// </summary>
    public static class TestHelper
    {
        public static async Task DeletePatientAsync(string workspaceId, string mrn)
        {
            var proKnow = TestSettings.ProKnow;
            var patientSummary = await proKnow.Patients.FindAsync(workspaceId, t => t.Mrn == mrn);
            if (patientSummary != null)
            {
                await proKnow.Patients.DeleteAsync(workspaceId, patientSummary.Id);
                while (await proKnow.Patients.FindAsync(workspaceId, p => p.Mrn == mrn) != null)
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
