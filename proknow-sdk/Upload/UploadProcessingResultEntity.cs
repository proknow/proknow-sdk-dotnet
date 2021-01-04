using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// The entity data for an element in the response from an upload processing query
    /// </summary>
    public class UploadProcessingResultEntity
    {
        /// <summary>
        /// The entity ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The entity UID
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// The entity type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The entity modality
        /// </summary>
        [JsonPropertyName("modality")]
        public string Modality { get; set; }

        /// <summary>
        /// The entity description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
