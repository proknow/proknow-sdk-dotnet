using System.Text.Json.Serialization;

using ProKnow.Patient;

namespace ProKnow.Upload
{
    /// <summary>
    /// Overrides to be applied to an uploaded file
    /// </summary>
    public class UploadFileOverrides
    {
        /// <summary>
        /// Patient overrides to be applied to an uploaded file
        /// </summary>
        [JsonPropertyName("patient")]
        public PatientMetadata Patient { get; set; }
    }
}
