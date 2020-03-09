using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// The properties for representing a custom metric in a scorecard template
    /// </summary>
    public class CustomMetricIdSchema
    {
        /// <summary>
        /// The ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
