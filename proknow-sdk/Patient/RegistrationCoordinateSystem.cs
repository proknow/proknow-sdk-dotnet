using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// The source or target of a registration object
    /// </summary>
    public class RegistrationCoordinateSystem
    {
        /// <summary>
        /// The frame of reference UID
        /// </summary>
        [JsonPropertyName("frame_of_reference")]
        public string FrameOfReferenceUid { get; set; }

        /// <summary>
        /// The image set ProKnow ID
        /// </summary>
        [JsonPropertyName("image_set")]
        public string ImageSetId { get; set; }
    }
}
