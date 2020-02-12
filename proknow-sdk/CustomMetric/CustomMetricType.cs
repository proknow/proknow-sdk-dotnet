using System.Text.Json.Serialization;

namespace ProKnow.CustomMetric
{
    /// <summary>
    /// The base class for custom metric types
    /// </summary>
    public class CustomMetricType
    {
        private const string COMMA = ", ";

        /// <summary>
        /// If not null, indicates an enum type
        /// </summary>
        [JsonPropertyName("enum")]
        public CustomMetricEnum Enum { get; set; }

        /// <summary>
        /// If not null, indicates a number type
        /// </summary>
        [JsonPropertyName("number")]
        public object Number { get; set; }

        /// <summary>
        /// If not null, indicates a string type
        /// </summary>
        [JsonPropertyName("string")]
        public object String { get; set; }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Enum != null)
            {
                return $"Enum: ({string.Join(COMMA, Enum.Values)})";
            }
            else if (Number != null)
            {
                return "Number";
            }
            else if (String != null)
            {
                return "String";
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
