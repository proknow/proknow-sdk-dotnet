using System.Text.Json.Serialization;

namespace ProKnow.Upload
{
    /// <summary>
    /// Patient properties to override in a DICOM upload
    /// </summary>
    public class PatientOverridesSchema
    {
        /// <summary>
        /// The ProKnow ID for the patient or null.  If present, this will be used for matching to an existing patient
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The patient medical record number (MRN) or DICOM patient ID (required).  This will be used for matching
        /// to an existing patient if Id is not present
        /// </summary>
        [JsonPropertyName("mrn")]
        public string Mrn { get; set; }

        /// <summary>
        /// The patient name (required)
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
    }
}
