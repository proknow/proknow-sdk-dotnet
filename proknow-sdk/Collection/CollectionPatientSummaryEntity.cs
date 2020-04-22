using System.Text.Json.Serialization;

namespace ProKnow.Collection
{
    /// <summary>
    /// The entity properties in a collection patient summary
    /// </summary>
    public class CollectionPatientSummaryEntity
    {
        /// <summary>
        /// The ProKnow ID of the entity
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The entity type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
