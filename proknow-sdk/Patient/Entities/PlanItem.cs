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
        /// Downloads this plan asynchronously as a DICOM object to the specified folder or file
        /// </summary>
        /// <param name="path">The full path to the destination folder or file</param>
        /// <returns>The full path to the file to which the plan was downloaded</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// If the provided path is an existing folder, the plan will be saved to a file named
        /// RP.{SOP instance UID}.dcm.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the provided path is not an existing folder, the plan will be saved to the provided path.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
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
