using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// The properties required for a custom metric in a scorecard or template creation
    /// </summary>
    public class CustomMetric
    {
        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The objectives or null if not specified
        /// </summary>
        [JsonPropertyName("objectives")]
        public IList<MetricBin> Objectives { get; set; }

        /// <summary>
        /// Construct a CustomMetric object
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="objectives">The optional objectives</param>
        public CustomMetric(string name, IList<MetricBin> objectives = null)
        {
            Name = name;
            Objectives = objectives;
        }
    }
}
