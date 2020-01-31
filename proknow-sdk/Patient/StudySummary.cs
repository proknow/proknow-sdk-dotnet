using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProKnow.Patient.Entities;

namespace ProKnow.Patient
{
    /// <summary>
    /// Provides a summary of a study for a patient
    /// </summary>
    public class StudySummary
    {
        private Requestor _requestor;

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
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Entities within this study
        /// </summary>
        [JsonPropertyName("entities")]
        public IList<EntitySummary> Entities { get; set; }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="requestor">Issues requests to the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        internal void PostProcessDeserialization(Requestor requestor, string workspaceId, string patientId)
        {
            _requestor = requestor;
            WorkspaceId = workspaceId;
            PatientId = patientId;

            // Post-process deserialization of entities
            foreach (var entity in Entities)
            {
                entity.PostProcessDeserialization(requestor, workspaceId, patientId);
            }
        }
    }
}
