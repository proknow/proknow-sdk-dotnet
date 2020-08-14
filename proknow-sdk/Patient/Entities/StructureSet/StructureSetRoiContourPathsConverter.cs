using ProKnow.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Converts paths of 2D points in mm to and from their JSON representation as numeric arrays of consecutive X and Z coordinates in 1/1000 mm for each point in a path
    /// </summary>
    public class StructureSetRoiContourPathsConverter : JsonConverter<Point2D[][]>
    {
        /// <summary>
        /// Reads paths of 2D points in mm from their JSON representation as numeric arrays of consecutive X and Z coordinates in 1/1000 mm for each point in a path
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>A path of 2D points</returns>
        public override Point2D[][] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new ProKnowException($"Unexpected token parsing paths.  Expected StartArray, got {reader.TokenType}.");
            }
            var paths = new List<Point2D[]>();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray:
                        paths.Add(ReadPath(ref reader));
                        break;
                    case JsonTokenType.EndArray:
                        return paths.ToArray();
                    case JsonTokenType.Comment:
                        // skip
                        break;
                    default:
                        throw new ProKnowException($"Unexpected token when reading paths: {reader.TokenType}");
                }
            }
            throw new ProKnowException("Unexpected end when reading paths.");
        }

        /// <summary>
        /// Writes paths of 2D points in mm to their JSON representation as numeric arrays of consecutive X and Z coordinates in 1/1000 mm for each point in a path
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The path to convert</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, Point2D[][] value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var path in value)
            {
                if (path == null)
                {
                    continue;
                }

                writer.WriteStartArray();
                foreach (var point in path)
                {
                    writer.WriteNumberValue((int)Math.Round(1000 * point.X));
                    writer.WriteNumberValue((int)Math.Round(1000 * point.Z));
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }

        /// <summary>
        /// Reads a path of 2D points in mm from its JSON representation as a numeric array of consecutive X and Z coordinates in 1/1000 mm for each point in the path
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Point2D[] ReadPath(ref Utf8JsonReader reader)
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
                        if (coordinates.Count % 2 != 0)
                        {
                            throw new ProKnowException("The numeric array must have an even number of elements.");
                        }
                        var points = new List<Point2D>();
                        for (int i = 0; i < coordinates.Count; i += 2)
                        {
                            points.Add(new Point2D(0.001 * coordinates[i], 0.001 * coordinates[i + 1]));
                        }
                        return points.ToArray();
                    case JsonTokenType.Comment:
                        // skip
                        break;
                    default:
                        throw new ProKnowException($"Unexpected token when reading path coordinates: {reader.TokenType}");
                }
            }
            throw new ProKnowException("Unexpected end when reading path coordinates.");
        }
    }
}
