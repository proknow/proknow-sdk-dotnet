using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Represents a transformation for a spatial registration object
    /// </summary>
    public class SroTransformation
    {
        /// <summary>
        /// The 4x4 transformation matrix in row-major order
        /// </summary>
        [JsonPropertyName("matrix")]
        public double[] Matrix { get; set; }
    }
}
