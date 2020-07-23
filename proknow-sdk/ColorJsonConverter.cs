using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow
{
    /// <summary>
    /// Converts a color to and from its JSON representation as an numeric array with three red, green, and blue elements.
    /// </summary>
    public class ColorJsonConverter : JsonConverter<Color>
    {
        /// <summary>
        /// Reads a Color from a JSON numeric array representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>The color</returns>
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var byteList = new List<byte>();
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.Number:
                            byteList.Add((byte)reader.GetInt16());
                            break;
                        case JsonTokenType.EndArray:
                            if (byteList.Count != 3)
                            {
                                throw new Exception("The numeric array must have three elements.");
                            }
                            return Color.FromArgb(byteList[0], byteList[1], byteList[2]);
                        case JsonTokenType.Comment:
                            // skip
                            break;
                        default:
                            throw new Exception($"Unexpected token when reading bytes: {reader.TokenType}");
                    }
                }
                throw new Exception("Unexpected end when reading bytes.");
            }
            else
            {
                throw new Exception($"Unexpected token parsing binary.  Expected StartArray, got {reader.TokenType}.");
            }
        }

        /// <summary>
        /// Writes a Color in JSON numeric array representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The color to convert</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue((int)value.R);
            writer.WriteNumberValue((int)value.G);
            writer.WriteNumberValue((int)value.B);
            writer.WriteEndArray();
        }
    }
}
