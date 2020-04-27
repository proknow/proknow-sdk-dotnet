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
        /// Downloads this dose asynchronously as a DICOM object to the specified folder or file
        /// </summary>
        /// <param name="path">The full path to the destination folder or file</param>
        /// <returns>The full path to the file to which the dose was downloaded</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// If the provided path is an existing folder, the dose will be saved to a file named
        /// RD.{SOP instance UID}.dcm.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the provided path is not an existing folder, the dose will be saved to the provided path.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public override Task<string> DownloadAsync(string path)
        {
            string file = null;
            if (Directory.Exists(path))
            {
                file = Path.Combine(path, $"RD.{Uid}.dcm");
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
            return _proKnow.Requestor.StreamAsync($"/workspaces/{WorkspaceId}/doses/{Id}/dicom", file);
        }

        //todo--Implement GetSliceData
    }
}
