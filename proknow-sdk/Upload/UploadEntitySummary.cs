using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ProKnow.Patient;
using ProKnow.Patient.Entities;

namespace ProKnow.Upload
{
    /// <summary>
    /// A summary view of an entity in an upload response
    /// </summary>
    public class UploadEntitySummary
    {
        private Patients _patients;

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
        /// The entity ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Gets the complete representation of the entity
        /// </summary>
        /// <returns>A complete representation of the entity</returns>
        public Task<EntityItem> GetAsync()
        {
            var patientTask = _patients.GetAsync(WorkspaceId, PatientId);
            var entitySummaryTask = patientTask.ContinueWith(t => t.Result.FindEntities(e => e.Id == Id)[0]);
            return entitySummaryTask.ContinueWith(t => t.Result.GetAsync()).Unwrap();
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="patients">Interacts with patients in a ProKnow organization</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        internal void PostProcessDeserialization(Patients patients, string workspaceId, string patientId)
        {
            _patients = patients;
            WorkspaceId = workspaceId;
            PatientId = patientId;
        }
    }
}
