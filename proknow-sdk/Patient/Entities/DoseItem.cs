using System;
using System.IO;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a dose for a patient
    /// </summary>
    public class DoseItem : EntityItem
    {
        /// <summary>
        /// Downloads this entity asynchronously as a DICOM object to the specified folder
        /// </summary>
        /// <param name="folder">The full path to the destination root folder</param>
        /// <returns>The full path to the destination folder (root or a sub-folder) to which the file(s) were downloaded</returns>
        public override Task<string> DownloadAsync(string folder)
        {
            if (File.Exists(folder))
            {
                throw new ArgumentException($"The destination folder path '{folder}' is a path to an existing file.");
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var file = Path.Combine(folder, $"RD.{Uid}.dcm");
            return _requestor.StreamAsync($"/workspaces/{WorkspaceId}/doses/{Id}/dicom", file);
        }

        //todo--Implement GetSliceData
    }
}
