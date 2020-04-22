using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// The study data for an element in the response from an upload status query
    /// </summary>
    public class UploadStatusResultStudy
    {
        /// <summary>
        /// The study ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The study UID
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// The study name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
