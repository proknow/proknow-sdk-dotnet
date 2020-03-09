using System.Collections.Generic;
using System.Text.Json.Serialization;

using ProKnow.CustomMetric;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Properties for creating scorecard templates
    /// </summary>
    public class ScorecardTemplateCreateSchema
    {
        /// <summary>
        /// The ID (null for create request; returned in create response)
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The computed metrics
        /// </summary>
        [JsonPropertyName("computed")]
        public IList<ComputedMetric> ComputedMetrics { get; set; }

        /// <summary>
        /// The ProKnow IDs for the custom metrics
        /// </summary>
        [JsonPropertyName("custom")]
        public IList<CustomMetricIdSchema> CustomMetricIdSchemas { get; set; }
    }
}
