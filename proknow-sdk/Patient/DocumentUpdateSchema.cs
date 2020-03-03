using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Properties used to update a document
    /// </summary>
    public class DocumentUpdateSchema
    {
        /// <summary>
        /// The document name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The document category
        /// </summary>
        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}
