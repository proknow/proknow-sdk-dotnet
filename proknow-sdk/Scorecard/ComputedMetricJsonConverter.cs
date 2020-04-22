using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Converts a computed metric to and from its JSON representation
    /// </summary>
    public class ComputedMetricJsonConverter : JsonConverter<ComputedMetric>
    {
        private readonly JsonEncodedText _typeKey = JsonEncodedText.Encode("type");
        private readonly JsonEncodedText _roiNameKey = JsonEncodedText.Encode("roi_name");
        private readonly JsonEncodedText _arg1Key = JsonEncodedText.Encode("arg_1");
        private readonly JsonEncodedText _arg2Key = JsonEncodedText.Encode("arg_2");
        private readonly JsonEncodedText _objectivesKey = JsonEncodedText.Encode("objectives");

        /// <summary>
        /// Reads a computed metric from its JSON representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>A computed metric</returns>
        public override ComputedMetric Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string type = null;
            string roiName = null;
            double? maybeArg1 = null;
            double? maybeArg2 = null;
            IList<MetricBin> objectives = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Object start expected.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new ComputedMetric(type, roiName, maybeArg1, maybeArg2, objectives);
                }

                // Read property name
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Property name expected after object start.");
                }
                string propertyName = reader.GetString();

                // Read property value
                reader.Read();
                if (propertyName == "type")
                {
                    type = reader.GetString();
                }
                else if (propertyName == "roi_name")
                {
                    roiName = reader.GetString();
                }
                else if (propertyName == "arg_1")
                {
                    if (reader.TokenType != JsonTokenType.Null)
                    {
                        double arg1 = reader.GetDouble();
                        maybeArg1 = arg1;
                    }
                }
                else if (propertyName == "arg_2")
                {
                    if (reader.TokenType != JsonTokenType.Null)
                    {
                        double arg2 = reader.GetDouble();
                        maybeArg2 = arg2;
                    }
                }
                else if (propertyName == "objectives")
                {
                    objectives = JsonSerializer.Deserialize<IList<MetricBin>>(ref reader);
                }
                // ignore any other properties
            }
            throw new JsonException("End of JSON string reached before object end.");
        }

        /// <summary>
        /// Writes a computed metric in its JSON representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="computedMetric">The computed metric to write</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, ComputedMetric computedMetric, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // type (required)
            writer.WritePropertyName(_typeKey);
            writer.WriteStringValue(computedMetric.Type);

            // roi_name (required, even if null)
            writer.WritePropertyName(_roiNameKey);
            if (computedMetric.RoiName != null)
            {
                writer.WriteStringValue(computedMetric.RoiName);
            }
            else
            {
                writer.WriteNullValue();
            }

            // arg_1 (required, even if null)
            writer.WritePropertyName(_arg1Key);
            if (computedMetric.Arg1.HasValue)
            {
                writer.WriteNumberValue(computedMetric.Arg1.Value);
            }
            else
            {
                writer.WriteNullValue();
            }

            // arg_2 (required, even if null)
            writer.WritePropertyName(_arg2Key);
            if (computedMetric.Arg2.HasValue)
            {
                writer.WriteNumberValue(computedMetric.Arg2.Value);
            }
            else
            {
                writer.WriteNullValue();
            }

            // objectives
            if (computedMetric.Objectives != null)
            {
                writer.WritePropertyName(_objectivesKey);
                JsonSerializer.Serialize(writer, computedMetric.Objectives, typeof(IList<MetricBin>));
            }

            writer.WriteEndObject();
        }
    }
}
