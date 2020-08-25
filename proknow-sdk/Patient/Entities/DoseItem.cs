using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a dose for a patient
    /// </summary>
    public class DoseItem : EntityItem
    {
        /// <summary>
        /// Type-specific entity data
        /// </summary>
        [JsonPropertyName("data")]
        public DoseData Data { get; set; }

        /// <summary>
        /// JSON web token (JWT) for dose data
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

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
            string file;
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

        /// <summary>
        /// Gets the data for a specified slice asynchronously
        /// </summary>
        /// <param name="index">The slice index</param>
        /// <returns>The voxel data for the specified slice</returns>
        public async Task<UInt16[]> GetSliceDataAsync(int index)
        {
            var slice = Data.Slices[index];
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Key", Key) };
            var bytes = await _proKnow.Requestor.GetBinaryAsync($"/doses/{Id}/slices/{slice.Tag}", headerKeyValuePairs);
            if (bytes.Length % 2 != 0)
            {
                throw new ProKnowException("Dose slices should contain an even number of bytes.");
            }
            var sliceData = new UInt16[bytes.Length / 2];
            var i = 0;
            var j = 0;
            while (i < bytes.Length)
            {
                sliceData[j++] = (UInt16)((bytes[i++] << 8) | bytes[i++]);
            }
            return sliceData;
        }
    }
}
