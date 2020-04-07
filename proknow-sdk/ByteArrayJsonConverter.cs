using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow
{
    /// <summary>
    /// Converts a byte array to and from its JSON representation
    /// </summary>
    public class ByteArrayJsonConverter : JsonConverter<Byte[]>
    {
        /// <summary>
        /// Reads a byte array from a JSON numeric array representation
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>The byte array</returns>
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                            return byteList.ToArray();
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
        /// Writes a byte array in JSON numeric array representation
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The byte array to convert</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            for (var i = 0; i < value.Length; i++)
            {
                writer.WriteNumberValue((int)value[i]);
            }
            writer.WriteEndArray();
        }
    }
}
