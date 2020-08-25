using ProKnow.Geometry;
using ProKnow.JsonConverters;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents a contour for an ROI
    /// </summary>
    public class StructureSetRoiContour
    {
        /// <summary>
        /// The couch IEC y-coordinate of the slice containing this contour
        /// </summary>
        [JsonConverter(typeof(CoordinateJsonConverter))]
        [JsonPropertyName("pos")]
        public double Position { get; set; }

        /// <summary>
        /// The contour paths on this slice
        /// </summary>
        [JsonConverter(typeof(Paths2DJsonConverter))]
        [JsonPropertyName("paths")]
        public Point2D[][] Paths { get; set; }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return Position.ToString("0.###");
        }
    }
}
