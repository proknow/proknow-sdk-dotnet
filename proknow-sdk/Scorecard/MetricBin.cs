using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Defines a bin for metrics
    /// </summary>
    [JsonConverter(typeof(MetricBinJsonConverter))]
    public class MetricBin
    {
        /// <summary>
        /// The label
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        //todo--Use System.Drawing or System.Windows.Media for colors
        /// <summary>
        /// The RGB color values
        /// </summary>
        [JsonPropertyName("color")]
        [JsonConverter(typeof(ByteArrayJsonConverter))]
        public byte[] Color { get; set; }

        /// <summary>
        /// Indicates that metrics with values greater than or equal to this minimum value will be placed in this bin
        /// </summary>
        [JsonPropertyName("min")]
        public double? Min { get; set; }

        /// <summary>
        /// Indicates that metrics with values less than or equal to this maximum value will be placed in this bin
        /// </summary>
        [JsonPropertyName("max")]
        public double? Max { get; set; }

        /// <summary>
        /// Used by deserialization to create a metric bin
        /// </summary>
        public MetricBin()
        {
        }

        /// <summary>
        /// Constructs a metric bin
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="color">The RGB color values</param>
        /// <param name="min">indicates that metrics with values greater than or equal to this minimum value will be
        /// placed in this bin</param>
        /// <param name="max">indicates that metrics with values less than or equal to this maximum value will be
        /// placed in this bin</param>
        public MetricBin(string label, byte[] color, double? min = null, double? max = null)
        {
            Label = label;
            Color = color;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Provides a string representation of this instance
        /// </summary>
        /// <returns>A string representation of this instance</returns>
        public override string ToString()
        {
            var color = $" | [{Color[0]}, {Color[1]}, {Color[2]}]";
            var min = Min != null ? $" | {Min}" : "";
            var max = Max != null ? $" | {Max}" : "";
            return $"{Label}{color}{min}{max}";
        }
    }
}
