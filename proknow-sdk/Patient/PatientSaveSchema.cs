using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Properties used to save a patient
    /// </summary>
    public class PatientSaveSchema
    {
        /// <summary>
        /// The patient medical record number (MRN) or ID
        /// </summary>
        [JsonPropertyName("mrn")]
        public string Mrn { get; set; }

        /// <summary>
        /// The patient name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The patient birth date in the format "YYYY-MM-DD" or null
        /// </summary>
        [JsonPropertyName("birth_date")]
        public string BirthDate { get; set; }

        /// <summary>
        /// The patient sex, one of "M", "F", "O" or null
        /// </summary>
        [JsonPropertyName("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// The patient metadata (custom metrics)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }
}
