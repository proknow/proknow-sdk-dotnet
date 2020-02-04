using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ProKnow.Patient;

namespace ProKnow.Upload
{
    /// <summary>
    /// A summary view of a patient in an upload response
    /// </summary>
    public class UploadPatientSummary
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
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Entities within this patient
        /// </summary>
        [JsonIgnore]
        public IList<UploadEntitySummary> Entities { get; set; }

        /// <summary>
        /// Gets the complete representation of the patient
        /// </summary>
        /// <returns></returns>
        public Task<PatientItem> GetAsync()
        {
            return _patients.GetAsync(WorkspaceId, Id);
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="patients">Interacts with patients in a ProKnow organization</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal void PostProcessDeserialization(Patients patients, string workspaceId)
        {
            _patients = patients;
            WorkspaceId = workspaceId;
            Entities = new List<UploadEntitySummary>();
        }
    }
}
