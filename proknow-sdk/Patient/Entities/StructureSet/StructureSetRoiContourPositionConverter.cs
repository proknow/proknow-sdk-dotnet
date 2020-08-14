using ProKnow.Exceptions;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Converts a contour position in mm to and from its JSON representation as a numeric value in 1/1000 mm
    /// </summary>
    internal class StructureSetRoiContourPositionConverter : JsonConverter<double>
    {
        /// <summary>
        /// Reads a contour position in mm from its JSON representation as a numeric value in 1/1000 mm
        /// </summary>
        /// <param name="reader">The JSON reader</param>
        /// <param name="typeToConvert">The type to convert</param>
        /// <param name="options">The serializer options</param>
        /// <returns>The contour position in mm</returns>
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new ProKnowException($"Unexpected token parsing contour position.  Expected Number, got {reader.TokenType}.");
            }
            return 0.001 * reader.GetInt32();
        }

        /// <summary>
        /// Writes a contour position in mm to its JSON representation as a numeric value in 1/1000 mm
        /// </summary>
        /// <param name="writer">The JSON writer</param>
        /// <param name="value">The path to convert</param>
        /// <param name="options">The JSON serializer options</param>
        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)Math.Round(1000 * value));
        }
    }
}
