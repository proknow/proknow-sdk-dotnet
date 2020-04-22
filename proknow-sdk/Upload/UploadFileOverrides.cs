using ProKnow.Patient;
using System.Text.Json.Serialization;

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
        public PatientCreateSchema Patient { get; set; }
    }
}
