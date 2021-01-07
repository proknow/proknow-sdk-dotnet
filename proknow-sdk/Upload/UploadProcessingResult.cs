using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// An element in the response from an upload processing query
    /// </summary>
    public class UploadProcessingResult
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
        /// The processing status:
        /// "completed":  This object successfully completed processing
        /// "pending":  This object needs attention due to a conflict
        /// "failed":  This object failed to process
        /// "processing":  This object did not complete processing before retry delays were exhausted
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
        public UploadProcessingResultPatient Patient { get; set; }

        /// <summary>
        /// The study information
        /// </summary>
        [JsonPropertyName("study")]
        public UploadProcessingResultStudy Study { get; set; }

        /// <summary>
        /// The entity information
        /// </summary>
        [JsonPropertyName("entity")]
        public UploadProcessingResultEntity Entity { get; set; }

        /// <summary>
        /// The SRO information
        /// </summary>
        [JsonPropertyName("sro")]
        public UploadProcessingResultSro Sro { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return $"{Id} {Path} {Status} {UpdatedAt}";
        }
    }
}
