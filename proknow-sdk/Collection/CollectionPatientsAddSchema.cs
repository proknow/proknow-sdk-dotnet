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
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The optional ProKnow ID for the entity
        /// </summary>
        [JsonPropertyName("entity_id")]
        public string EntityId { get; set; }

        /// <summary>
        /// Creates a CollectionPatientProperties object
        /// </summary>
        /// <param name="id">The ProKnow ID for the patient</param>
        /// <param name="entityId">The optional ProKnow ID for the entity</param>
        public CollectionPatientsAddSchema(string id, string entityId = null)
        {
            Id = id;
            EntityId = entityId;
        }
    }
}
