using System.Collections.Generic;
using System.Text.Json.Serialization;

using ProKnow.CustomMetric;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Properties for saving changes to scorecard templates
    /// </summary>
    public class ScorecardTemplateSaveSchema
    {
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
        /// The custom metrics
        /// </summary>
        [JsonPropertyName("custom")]
        public IList<CustomMetricItem> CustomMetrics { get; set; }
    }
}
