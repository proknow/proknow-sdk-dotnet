using ProKnow.Geometry;
using ProKnow.JsonConverters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents the contour and point data for an ROI
    /// </summary>
    public class StructureSetRoiData
    {
        /// <summary>
        /// The contours
        /// </summary>
        [JsonPropertyName("contours")]
        public StructureSetRoiContour[] Contours { get; set; }

        /// <summary>
        /// The points
        /// </summary>
        [JsonPropertyName("points")]
        [JsonConverter(typeof(Points3DJsonConverter))]
        public Point3D[] Points { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }

    //todo--Add IsEditable()

    //todo--Add SaveAsync()
}
