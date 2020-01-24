using System.Collections.Generic;
using System.Text.Json.Serialization;
using ProKnow.Patient.Entities;

namespace ProKnow.Patient
{
    /// <summary>
    /// Provides a summary of a study for a patient
    /// </summary>
    public class StudySummary
    {
        /// <summary>
        /// The parent Patients object
        /// </summary>
        [JsonIgnore]
        internal Patients Patients { get; set; }

        /// <summary>
        /// The workspace ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonIgnore]
        public string PatientId { get; internal set; }

        /// <summary>
        /// The study ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// All study summary attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Entities within this study
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
