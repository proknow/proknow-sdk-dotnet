using System;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a plan for a patient
    /// </summary>
    public class PlanItem : EntityItem
    {
        /// <summary>
        /// Downloads this entity asynchronously as a DICOM object to a specified folder or file
        /// </summary>
        /// <param name="path">The full path to the destination folder or file</param>
        /// <returns>The full path to the file downloaded</returns>
        public override Task<string> DownloadAsync(string path)
        {
            string file = null;
            if (Directory.Exists(path))
            {
                file = Path.Combine(path, $"RP.{Uid}.dcm");
            }
            else
            {
                var parentDirectoryInfo = Directory.GetParent(path);
                if (!parentDirectoryInfo.Exists)
                {
                    parentDirectoryInfo.Create();
                }
                file = path;
            }
            return _proKnow.Requestor.StreamAsync($"/workspaces/{WorkspaceId}/plans/{Id}/dicom", file);
        }
    }
}
