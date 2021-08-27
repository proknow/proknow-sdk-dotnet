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
        /// JSON web token (JWT) for image set data
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// Type-specific entity data
        /// </summary>
        [JsonPropertyName("data")]
        public ImageSetData Data { get; set; }

        /// <summary>
        /// Downloads this image set asynchronously as DICOM objects to the specified folder
        /// </summary>
        /// <param name="path">The full path to the destination folder</param>
        /// <returns>The full path to the folder to which the images were downloaded</returns>
        /// <remarks>
        /// The provided path must be an existing folder.  A subfolder named {modality}.{series instance UID} will be
        /// created in this folder and the individual images will be saved to files named
        /// {modality}.{SOP instance UID}.dcm where {modality} is an abbreviation of the modality.
        /// </remarks>
        public override async Task<string> DownloadAsync(string path)
        {
            // Create destination folder, if necessary
            var folder = Path.Combine(path, $"{Modality}.{Uid}");
            if (File.Exists(folder))
            {
                throw new ArgumentException($"The image set download folder path '{path}' is a path to an existing file.");
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Download each image to the destination folder asynchronously
            var tasks = new List<Task<string>>();
            foreach (var image in Data.Images)
            {
                var file = Path.Combine(folder, $"{Modality}.{image.Uid}.dcm");
                var route = $"/workspaces/{WorkspaceId}/imagesets/{Id}/images/{image.Id}/dicom";
                tasks.Add(Task.Run(() => _proKnow.Requestor.StreamAsync(route, file)));
            }
            await Task.WhenAll(tasks);

            // Return the destination folder
            return folder;
        }

        /// <summary>
        /// Gets the pixel data for a specified image asynchronously
        /// </summary>
        /// <param name="index">The index of the image</param>
        /// <returns>The pixel data for the specified image</returns>
        /// <example>This example shows how to get the pixel data for an image:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var workspaceItem = await _proKnow.Workspaces.FindAsync(w => w.Name == "Clinical");
        /// var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Name == "Example");
        /// var patientItem = await patientSummary.GetAsync();
        /// var imageSetItem = patientItem.FindEntities(e => e.Type == "image_set")[0];
        /// var imageData = await imageSetItem.GetImageDataAsync(0);
        /// </code>
        /// </example>
        public async Task<UInt16[]> GetImageDataAsync(int index)
        {
            var image = Data.Images[index];
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Key", Key) };
            var bytes = await _proKnow.Requestor.GetBinaryAsync($"/imagesets/{Id}/images/{image.Tag}", headerKeyValuePairs);
            var imageData = new UInt16[bytes.Length / 2];
            var i = 0;
            var j = 0;
            while (i < bytes.Length)
            {
                imageData[j++] = (UInt16)((bytes[i++] << 8) | bytes[i++]); // Convert from network byte order (big endian)
            }
            return imageData;
        }
    }
}
