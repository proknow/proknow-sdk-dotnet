using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// Represents the response from a request to initiate a file upload
    /// </summary>
    public class InitiateFileUploadResponse
    {
        /// <summary>
        /// The ProKnow ID for the upload
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The file checksum (MD5 hash)
        /// </summary>
        [JsonPropertyName("checksum")]
        public string Checksum { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// The full path to the file
        /// </summary>
        [JsonPropertyName("name")]
        public string Path { get; set; }

        /// <summary>
        /// The file size
        /// </summary>
        [JsonPropertyName("size")]
        public long Filesize { get; set; }

        /// <summary>
        /// Indicates whether the upload is multipart
        /// </summary>
        [JsonPropertyName("multipart")]
        public bool IsMultipart { get; set; }

        /// <summary>
        /// The key for the upload
        /// </summary>
        [JsonPropertyName("key")]
        public string Key { get; set; }

        /// <summary>
        /// The identifier for the upload
        /// </summary>
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
