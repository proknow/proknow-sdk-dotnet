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
        /// Used by deserialization to create custom metric item
        /// </summary>
        protected CustomMetricItem()
        {
        }

        /// <summary>
        /// Used by deserialization to create custom metric item
        /// </summary>
        /// <param name="id">The ProKnow ID</param>
        /// <param name="name">The name</param>
        /// <param name="context">The context</param>
        /// <param name="type">The type</param>
        internal CustomMetricItem(string id, string name, string context, CustomMetricType type)
        {
            Id = id;
            Name = name;
            Context = context;
            Type = type;
        }

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
                Id = Id
            };
        }
    }
}
