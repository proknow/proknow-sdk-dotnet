using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.CustomMetric
{
    /// <summary>
    /// Represents a custom metric
    /// </summary>
    public class CustomMetricItem
    {
        private CustomMetrics _customMetrics;
        private Requestor _requestor;

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
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create custom metric item
        /// </summary>
        protected CustomMetricItem()
        {
        }

        /// <summary>
        /// Creates a custom metric type
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="context">The context</param>
        /// <param name="type">The type</param>
        public CustomMetricItem(string name, string context, CustomMetricType type)
        {
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
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="customMetrics">The parent CustomMetrics object</param>
        /// <param name="requestor">Issues requests to the ProKnow API</param>
        internal void PostProcessDeserialization(CustomMetrics customMetrics, Requestor requestor)
        {
            _customMetrics = customMetrics;
            _requestor = requestor;
        }
    }
}
