using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Provides a summary of an entity for a study
    /// </summary>
    public class EntitySummary
    {
        /// <summary>
        /// The parent Patients object
        /// </summary>
        [JsonIgnore]
        internal Patients Patients { get; set; }

        /// <summary>
        /// The patient workspace ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonIgnore]
        public string PatientId { get; internal set; }

        /// <summary>
        /// The entity ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// All entity summary attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Entities within this entity
        /// </summary>
        [JsonPropertyName("entities")]
        public IList<EntitySummary> Entities { get; set; }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="patients">The parent Patients object</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        internal void PostProcessDeserialization(Patients patients, string workspaceId, string patientId)
        {
            Patients = patients;
            WorkspaceId = workspaceId;
            PatientId = patientId;

            // Post-process deserialization of entities
            foreach (var entity in Entities)
            {
                entity.PostProcessDeserialization(patients, workspaceId, patientId);
            }

            // Add member properties to collection of deserialized properties that had no matching member
            Data.Add("id", Id);
            Data.Add("entities", Entities);
        }
    }
}
