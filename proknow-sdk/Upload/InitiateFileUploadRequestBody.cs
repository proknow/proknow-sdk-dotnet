using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// The body for a file upload request
    /// </summary>
    internal class InitiateFileUploadRequestBody
    {
        /// <summary>
        /// The file checksum (MD5 hash)
        /// </summary>
        [JsonPropertyName("checksum")]
        public string Checksum { get; set; }

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
        /// Overrides to be applied to the uploaded file
        /// </summary>
        [JsonPropertyName("overrides")]
        public UploadFileOverrides Overrides { get; set; }
    }
}
