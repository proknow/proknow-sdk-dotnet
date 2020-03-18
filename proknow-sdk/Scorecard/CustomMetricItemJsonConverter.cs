using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Converts a custom metric to and from its JSON representation
    /// </summary>
    public class CustomMetricItemJsonConverter : JsonConverter<CustomMetricItem>
    {
        private readonly JsonEncodedText _idKey = JsonEncodedText.Encode("id");
        private readonly JsonEncodedText _nameKey = JsonEncodedText.Encode("name");
        private readonly JsonEncodedText _contextKey = JsonEncodedText.Encode("context");
        private readonly JsonEncodedText _typeKey = JsonEncodedText.Encode("type");
        private readonly JsonEncodedText _objectivesKey = JsonEncodedText.Encode("objectives");

        /// <summary>
        /// Reads a custom metric item from its JSON representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>A custom metric</returns>
        public override CustomMetricItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string id = null;
            string name = null;
            string context = null;
            CustomMetricType type = null;
            IList<MetricBin> objectives = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Object start expected.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new CustomMetricItem(id, name, context, type, objectives);
                }

                // Read property name
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Property name expected after object start.");
                }
                string propertyName = reader.GetString();

                // Read property value
                reader.Read();
                if (propertyName == "id")
                {
                    id = reader.GetString();
                }
                else if (propertyName == "name")
                {
                    name = reader.GetString();
                }
                else if (propertyName == "context")
                {
                    context = reader.GetString();
                }
                else if (propertyName == "type")
                {
                    type = JsonSerializer.Deserialize<CustomMetricType>(ref reader);
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
        /// Writes a custom metric item in its JSON representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="customMetricItem">The custom metric item to write</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, CustomMetricItem customMetricItem, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // id
            if (customMetricItem.Id != null)
            {
                writer.WritePropertyName(_idKey);
                writer.WriteStringValue(customMetricItem.Id);
            }

            // name
            if (customMetricItem.Name != null)
            {
                writer.WritePropertyName(_nameKey);
                writer.WriteStringValue(customMetricItem.Name);
            }

            // context
            if (customMetricItem.Context != null)
            {
                writer.WritePropertyName(_contextKey);
                writer.WriteStringValue(customMetricItem.Context);
            }

            // type
            if (customMetricItem.Type != null)
            {
                writer.WritePropertyName(_typeKey);
                var overrideOptions = new JsonSerializerOptions { IgnoreNullValues = true };
                JsonSerializer.Serialize(writer, customMetricItem.Type, overrideOptions);
            }

            // objectives
            if (customMetricItem.Objectives != null)
            {
                writer.WritePropertyName(_objectivesKey);
                JsonSerializer.Serialize(writer, customMetricItem.Objectives, typeof(IList<MetricBin>));
            }

            writer.WriteEndObject();
        }
    }
}
