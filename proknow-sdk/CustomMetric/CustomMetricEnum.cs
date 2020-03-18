using System.Linq;
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

        /// <summary>
        /// Used by deserialization to create a custom metric enum
        /// </summary>
        protected CustomMetricEnum()
        {
        }

        /// <summary>
        /// Creates a custom metric enum
        /// </summary>
        /// <param name="values">The enum values</param>
        public CustomMetricEnum(string[] values)
        {
            Values = values.ToArray();
        }
    }
}
