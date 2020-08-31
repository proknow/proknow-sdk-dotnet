using ProKnow.JsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// A container for image data
    /// </summary>
    public class Image
    {
        /// <summary>
        /// The image ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The intercept of the linear transformation from stored pixel value to output units
        /// </summary>
        [JsonPropertyName("b")]
        public double RescaleIntercept { get; set; }

        /// <summary>
        /// The slope of the linear transformation from stored pixel value to output units
        /// </summary>
        [JsonPropertyName("m")]
        public double RescaleSlope { get; set; }

        /// <summary>
        /// The image position in mm
        /// </summary>
        [JsonConverter(typeof(CoordinateJsonConverter))]
        [JsonPropertyName("pos")]
        public double Position { get; set; }

        /// <summary>
        /// The IEC X image position in mm
        /// </summary>
        [JsonPropertyName("pos_x")]
        public double PositionX { get; set; }

        /// <summary>
        /// The IEC Y image position in mm
        /// </summary>
        [JsonPropertyName("pos_y")]
        public double PositionY { get; set; }

        /// <summary>
        /// The IEC Z image position in mm
        /// </summary>
        [JsonPropertyName("pos_z")]
        public double PositionZ { get; set; }

        /// <summary>
        /// The tag for the pixel data in blob storage
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// The SOP instance UID
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

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
