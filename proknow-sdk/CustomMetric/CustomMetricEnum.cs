using System;
using System.Text.Json.Serialization;

namespace ProKnow.CustomMetric
{
    /// <summary>
    /// Represents a custom metric enum
    /// </summary>
    public class CustomMetricEnum
    {
        /// <summary>
        /// The values
        /// </summary>
        [JsonPropertyName("values")]
        public string[] Values { get; set; }
    }
}
