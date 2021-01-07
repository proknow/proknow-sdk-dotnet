using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// The result of an upload
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// The ProKnow ID for the upload
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The full path to the uploaded file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The upload status:
        /// "uploading":  The file is currently being processed
        /// "completed":  This object has previously been uploaded and successfully completed processing
        /// "pending":  This object has previously been uploaded, but needs attention due to a conflict
        /// "failed":  This object has previously been uploaded, but failed to process
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Constructs an UploadResult
        /// </summary>
        /// <param name="id">The ProKnow ID for the upload</param>
        /// <param name="path">The full path to the uploaded file</param>
        /// <param name="status">The upload status</param>
        public UploadResult(string id, string path, string status)
        {
            Id = id;
            Path = path;
            Status = status;
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return $"{Id} {Path} {Status}";
        }
    }
}
