using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// An element in the response from an upload status query
    /// </summary>
    public class UploadStatusResult
    {
        /// <summary>
        /// The ProKnow ID for the upload
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The full path to the uploaded file
        /// </summary>
        [JsonPropertyName("name")]
        public string Path { get; set; }

        /// <summary>
        /// The uploaded filesize in bytes
        /// </summary>
        [JsonPropertyName("size")]
        public long Filesize { get; set; }

        /// <summary>
        /// The upload status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// A number indicating when the upload was last updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public long UpdatedAt { get; set; }

        /// <summary>
        /// The patient information
        /// </summary>
        [JsonPropertyName("patient")]
        public UploadStatusResultPatient Patient { get; set; }

        /// <summary>
        /// The study information
        /// </summary>
        [JsonPropertyName("study")]
        public UploadStatusResultStudy Study { get; set; }

        /// <summary>
        /// The entity information
        /// </summary>
        [JsonPropertyName("entity")]
        public UploadStatusResultEntity Entity { get; set; }

        /// <summary>
        /// The SRO information
        /// </summary>
        [JsonPropertyName("sro")]
        public UploadStatusResultSro Sro { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
