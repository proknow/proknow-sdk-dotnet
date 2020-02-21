using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents an image set for a patient
    /// </summary>
    public class ImageSetItem : EntityItem
    {
        /// <summary>
        /// Type-specific entity data
        /// </summary>
        [JsonPropertyName("data")]
        public ImageSetData Data { get; set; }

        /// <summary>
        /// Downloads this image set asynchronously as DICOM objects to the specified folder
        /// </summary>
        /// <param name="root">The full path to the destination root folder</param>
        /// <returns>The full path to the destination sub-folder to which the images were downloaded</returns>
        public override Task<string> DownloadAsync(string root)
        {
            var folder = Path.Combine(root, $"{Modality}.{Uid}");
            if (File.Exists(folder))
            {
                throw new ArgumentException($"The image set download folder path '{root}' is a path to an existing file.");
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var tasks = new List<Task<string>>();
            foreach (var image in Data.Images)
            {
                var file = Path.Combine(folder, $"{Modality}.{image.Uid}");
                tasks.Add(Task.Run(() => _requestor.StreamAsync($"/workspaces/{WorkspaceId}/imagesets/{Id}/images/{image.Id}/dicom", file)));
            }
            return Task.WhenAll(tasks).ContinueWith(t => folder);
        }
    }
}
