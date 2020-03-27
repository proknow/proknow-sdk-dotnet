using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Represents a custom metric
    /// </summary>
    [JsonConverter(typeof(CustomMetricItemJsonConverter))]
    public class CustomMetricItem
    {
        /// <summary>
        /// The ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The context
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; set; }

        /// <summary>
        /// The type
        /// </summary>
        [JsonPropertyName("type")]
        public CustomMetricType Type { get; set; }

        /// <summary>
        /// The objectives or null if not specified
        /// </summary>
        [JsonPropertyName("objectives")]
        public IList<MetricBin> Objectives { get; set; }

        /// <summary>
        /// Used by deserialization to create custom metric item
        /// </summary>
        public CustomMetricItem()
        {
        }

        /// <summary>
        /// Creates a custom metric for a scorecard
        /// </summary>
        /// <param name="name">The custom metric name (must be an existing custom metric)</param>
        /// <param name="objectives">The optional objectives</param>
        public CustomMetricItem(string name, IList<MetricBin> objectives = null)
        {
            Name = name;
            Objectives = objectives;
        }

        /// <summary>
        /// Used by deserialization to a create custom metric item
        /// </summary>
        /// <param name="id">The ProKnow ID</param>
        /// <param name="name">The name</param>
        /// <param name="context">The context</param>
        /// <param name="type">The type</param>
        /// <param name="objectives">The objectives or null if not specified</param>
        public CustomMetricItem(string id, string name, string context, CustomMetricType type,
            IList<MetricBin> objectives = null)
        {
            Id = id;
            Name = name;
            Context = context;
            Type = type;
            Objectives = objectives;
        }

        //todo--DeleteAsync (HIGH PRIORITY)

        //todo--SaveAsync (HIGH PRIORITY)

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Provide a copy of this instance containing only the information required to represent it in a scorecard
        /// create or save request
        /// </summary>
        /// <returns>A copy of this instance containing only the information required to represent it in a scorecard
        /// create or save request</returns>
        internal CustomMetricItem ConvertToScorecardSchema()
        {
            return new CustomMetricItem()
            {
                Id = Id,
                Objectives = Objectives
            };
        }
    }
}
