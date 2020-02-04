using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// The result of a file upload
    /// </summary>
    public class UploadFileResult
    {
        /// <summary>
        /// The full path to the file uploaded
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; }

        /// <summary>
        /// The upload response
        /// </summary>
        [JsonPropertyName("upload")]
        public InitiateFileUploadResponse Response { get; set; }
    }
}
