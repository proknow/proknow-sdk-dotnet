using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Converts a metric bin to and from its JSON representation
    /// </summary>
    public class MetricBinJsonConverter : JsonConverter<MetricBin>
    {
        private readonly JsonEncodedText _labelKey = JsonEncodedText.Encode("label");
        private readonly JsonEncodedText _colorKey = JsonEncodedText.Encode("color");
        private readonly JsonEncodedText _minKey = JsonEncodedText.Encode("min");
        private readonly JsonEncodedText _maxKey = JsonEncodedText.Encode("max");
        private readonly ByteArrayJsonConverter _byteArrayJsonConverter = new ByteArrayJsonConverter();

        /// <summary>
        /// Reads a metric bin from its JSON representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>A metric bin</returns>
        public override MetricBin Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string label = null;
            byte[] color = null;
            double? min = null;
            double? max = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Object start expected.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new MetricBin(label, color, min, max);
                }

                // Read property name
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Property name expected after object start.");
                }
                string propertyName = reader.GetString();

                // Read property value
                reader.Read();
                if (propertyName == "label")
                {
                    label = reader.GetString();
                }
                else if (propertyName == "color")
                {
                    color = _byteArrayJsonConverter.Read(ref reader, typeof(byte[]), options);
                }
                else if (propertyName == "min")
                {
                    min = reader.GetDouble();
                }
                else if (propertyName == "max")
                {
                    max = reader.GetDouble();
                }
                // ignore any other properties
            }
            throw new JsonException("End of JSON string reached before object end.");
        }

        /// <summary>
        /// Writes a metric bin in its JSON representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="metricBin">The metric bin to write</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, MetricBin metricBin, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // label
            writer.WritePropertyName(_labelKey);
            writer.WriteStringValue(metricBin.Label);

            // color
            writer.WritePropertyName(_colorKey);
            _byteArrayJsonConverter.Write(writer, metricBin.Color, options);

            // min
            if (metricBin.Min.HasValue)
            {
                writer.WriteNumber(_minKey, metricBin.Min.Value);
            }

            // max
            if (metricBin.Max.HasValue)
            {
                writer.WriteNumber(_maxKey, metricBin.Max.Value);
            }

            writer.WriteEndObject();
        }
    }
}
