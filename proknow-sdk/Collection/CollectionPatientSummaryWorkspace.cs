using System.Text.Json.Serialization;

namespace ProKnow.Collection
{
    /// <summary>
    /// The workspace properties in a collection patient summary
    /// </summary>
    public class CollectionPatientSummaryWorkspace
    {
        /// <summary>
        /// The ProKnow ID of the workspace
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The workspace slug
        /// </summary>
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        /// <summary>
        /// The workspace name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
