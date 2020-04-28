using ProKnow.Patient;
using ProKnow.Patient.Entities;
using System.Threading.Tasks;

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
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        public string PatientId { get; internal set; }

        /// <summary>
        /// The entity ProKnow ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The entity UID
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// The entity type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The entity modality
        /// </summary>
        public string Modality { get; set; }

        /// <summary>
        /// The entity description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Used by de-serialization to construct an UploadEntitySummary
        /// </summary>
        public UploadEntitySummary()
        {
        }

        /// <summary>
        /// Creates an entity summary
        /// </summary>
        /// <param name="patients">Interacts with patients in a ProKnow organization</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        /// <param name="uploadStatusResult">The upload status result</param>
        internal UploadEntitySummary(Patients patients, string workspaceId, string patientId, UploadStatusResultEntity uploadStatusResult)
        {
            _patients = patients;
            WorkspaceId = workspaceId;
            PatientId = patientId;
            Id = uploadStatusResult.Id;
            Uid = uploadStatusResult.Uid;
            Type = uploadStatusResult.Type;
            Modality = uploadStatusResult.Modality;
            Description = uploadStatusResult.Description;
        }

        /// <summary>
        /// Gets the complete representation of the entity
        /// </summary>
        /// <returns>A complete representation of the entity</returns>
        public async Task<EntityItem> GetAsync()
        {
            var patientItem = await _patients.GetAsync(WorkspaceId, PatientId);
            var entitySummary = patientItem.FindEntities(e => e.Id == Id)[0];
            return await entitySummary.GetAsync();
        }
    }
}
