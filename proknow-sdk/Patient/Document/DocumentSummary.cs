using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Document
{
    /// <summary>
    /// Provides a summary of a document for a patient
    /// </summary>
    public class DocumentSummary
    {
        /// <summary>
        /// The ProKnow ID of the document
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the document
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The MIME type of the document
        /// </summary>
        [JsonPropertyName("mime")]
        public string Mime { get; set; }

        /// <summary>
        /// The category of the document
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }

        /// <summary>
        /// The size of the document in bytes
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// The creator of the document
        /// </summary>
        [JsonPropertyName("created_by")]
        public UserSummary CreatedBy { get; set; }

        /// <summary>
        /// The creation timestamp of the document
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
