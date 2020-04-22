using System.Text.Json.Serialization;

namespace ProKnow.Collection
{
    /// <summary>
    /// The patient properties in a collection patient summary
    /// </summary>
    public class CollectionPatientSummaryPatient
    {
        /// <summary>
        /// The ProKnow ID of the patient
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

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
    }
}
