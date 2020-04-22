using System.Text.Json.Serialization;

namespace ProKnow.Collection
{
    /// <summary>
    /// The properties required to add a patient to a collection
    /// </summary>
    public class CollectionPatientsAddSchema
    {
        /// <summary>
        /// The ProKnow ID for the patient
        /// </summary>
        [JsonPropertyName("patient")]
        public string PatientId { get; set; }

        /// <summary>
        /// The optional ProKnow ID for the entity
        /// </summary>
        [JsonPropertyName("entity")]
        public string EntityId { get; set; }

        /// <summary>
        /// Creates a CollectionPatientAddSchema object
        /// </summary>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <param name="entityId">The optional ProKnow ID for the entity</param>
        public CollectionPatientsAddSchema(string patientId, string entityId = null)
        {
            PatientId = patientId;
            EntityId = entityId;
        }
    }
}
