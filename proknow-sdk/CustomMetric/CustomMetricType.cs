using System;
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
        /// Creates a custom metric type
        /// </summary>
        /// <param name="type">The type ('enum', 'number', or 'string')</param>
        /// <param name="enumValues">The enum values or null for types 'number' or 'string'</param>
        public CustomMetricType(string type, string[] enumValues = null)
        {
            if (type == "enum")
            {
                if (enumValues == null || enumValues.Length == 0)
                {
                    throw new ArgumentException("Enum values must be provided for custom metric type 'enum'.");
                }
                Enum = new CustomMetricEnum(enumValues);
            }
            else if (type == "number")
            {
                Number = new object();
            }
            else if (type == "string")
            {
                String = new object();
            }
            else
            {
                throw new ArgumentOutOfRangeException("The custom metric type must be 'enum', 'number', or 'string'.");
            }
        }

        /// <summary>
        /// Used by deserialization to create a custom metric type
        /// </summary>
        public CustomMetricType()
        {
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
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
