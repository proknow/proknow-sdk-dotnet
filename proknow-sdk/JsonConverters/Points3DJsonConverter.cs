using ProKnow.Exceptions;
using ProKnow.Geometry;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.JsonConverters
{
    /// <summary>
    /// Converts 3D points in mm to and from their JSON representation as numeric arrays of X, Y and Z coordinates in 1/1000 mm for each point
    /// </summary>
    public class Points3DJsonConverter : JsonConverter<Point3D[]>
    {
        /// <summary>
        /// Reads 3D points in mm from their JSON representation as numeric arrays of X, Y and Z coordinates in 1/1000 mm for each point
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>A path of 2D points</returns>
        public override Point3D[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new ProKnowException($"Unexpected token parsing points.  Expected StartArray, got {reader.TokenType}.");
            }
            var points = new List<Point3D>();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray:
                        points.Add(ReadPoint(ref reader));
                        break;
                    case JsonTokenType.EndArray:
                        return points.ToArray();
                    case JsonTokenType.Comment:
                        // skip
                        break;
                    default:
                        throw new ProKnowException($"Unexpected token when reading points: {reader.TokenType}");
                }
            }
            throw new ProKnowException("Unexpected end when reading points.");
        }

        /// <summary>
        /// Writes 3D points in mm to their JSON representation as numeric arrays of X, Y and Z coordinates in 1/1000 mm for each point
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The path to convert</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, Point3D[] value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var point in value)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue((int)Math.Round(1000 * point.X));
                writer.WriteNumberValue((int)Math.Round(1000 * point.Y));
                writer.WriteNumberValue((int)Math.Round(1000 * point.Z));
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        /// <summary>
        /// Reads a 3D point in mm from its JSON representation as a numeric array of X, Y and Z coordinates in 1/1000 mm
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Point3D ReadPoint(ref Utf8JsonReader reader)
        {
            var coordinates = new List<int>();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Number:
                        coordinates.Add(reader.GetInt32());
                        break;
                    case JsonTokenType.EndArray:
                        if (coordinates.Count != 3)
                        {
                            throw new ProKnowException("The numeric array must have exactly three elements.");
                        }
                        return new Point3D(0.001 * coordinates[0], 0.001 * coordinates[1], 0.001 * coordinates[2]);
                    case JsonTokenType.Comment:
                        // skip
                        break;
                    default:
                        throw new ProKnowException($"Unexpected token when reading point coordinates: {reader.TokenType}");
                }
            }
            throw new ProKnowException("Unexpected end when reading point coordinates.");
        }
    }
}
