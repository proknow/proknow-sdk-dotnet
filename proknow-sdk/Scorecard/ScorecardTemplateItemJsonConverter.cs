using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Converts a scorecard template item to and from its JSON representation
    /// </summary>
    public class ScorecardTemplateItemJsonConverter : JsonConverter<ScorecardTemplateItem>
    {
        private readonly JsonEncodedText _idKey = JsonEncodedText.Encode("id");
        private readonly JsonEncodedText _nameKey = JsonEncodedText.Encode("name");
        private readonly JsonEncodedText _computedKey = JsonEncodedText.Encode("computed");
        private readonly JsonEncodedText _customKey = JsonEncodedText.Encode("custom");

        /// <summary>
        /// Reads a scorecard template item from its JSON representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>A scorecard template</returns>
        public override ScorecardTemplateItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string id = null;
            string name = null;
            IList<ComputedMetric> computed = null;
            IList<CustomMetricItem> custom = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Object start expected.");
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new ScorecardTemplateItem(id, name, computed, custom);
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
                else if (propertyName == "computed")
                {
                    computed = JsonSerializer.Deserialize<IList<ComputedMetric>>(ref reader);
                }
                else if (propertyName == "custom")
                {
                    custom = JsonSerializer.Deserialize<IList<CustomMetricItem>>(ref reader);
                }
                // ignore any other properties
            }
            throw new JsonException("End of JSON string reached before object end.");
        }

        /// <summary>
        /// Writes a scorecard template item in its JSON representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="scorecardTemplateItem">The scorecard template item to write</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, ScorecardTemplateItem scorecardTemplateItem, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // id
            if (scorecardTemplateItem.Id != null)
            {
                writer.WritePropertyName(_idKey);
                writer.WriteStringValue(scorecardTemplateItem.Id);
            }

            // name
            if (scorecardTemplateItem.Name != null)
            {
                writer.WritePropertyName(_nameKey);
                writer.WriteStringValue(scorecardTemplateItem.Name);
            }

            // computed
            if (scorecardTemplateItem.ComputedMetrics != null)
            {
                writer.WritePropertyName(_computedKey);
                var overrideOptions = new JsonSerializerOptions { IgnoreNullValues = true };
                JsonSerializer.Serialize(writer, scorecardTemplateItem.ComputedMetrics, overrideOptions);
            }

            // custom
            if (scorecardTemplateItem.CustomMetrics != null)
            {
                // only include IDs
                var customMetricIds = scorecardTemplateItem.CustomMetrics.Select(c => c.ConvertToScorecardSchema()).ToList();
                writer.WritePropertyName(_customKey);
                var overrideOptions = new JsonSerializerOptions { IgnoreNullValues = true };
                JsonSerializer.Serialize(writer, customMetricIds, overrideOptions);
            }

            writer.WriteEndObject();
        }
    }
}
