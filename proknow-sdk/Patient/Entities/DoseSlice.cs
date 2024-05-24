using ProKnow.JsonConverters;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// A container for dose slice data
    /// </summary>
    public class DoseSlice
    {
        /// <summary>
        /// The slice position in mm
        /// </summary>
        [JsonConverter(typeof(CoordinateJsonConverter))]
        [JsonPropertyName("pos")]
        public double Position { get; set; }

        /// <summary>
        /// The id for the slice data in blob storage
        /// </summary>
        [JsonPropertyName("id")]
        public string Tag { get; set; }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return Position.ToString();
        }
    }
}
