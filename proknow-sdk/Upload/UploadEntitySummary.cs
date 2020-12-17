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
        private readonly Patients _patients;

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
        /// <param name="uploadProcessingResultEntity">The entity data of an upload processing result</param>
        internal UploadEntitySummary(Patients patients, string workspaceId, string patientId, UploadProcessingResultEntity uploadProcessingResultEntity)
        {
            _patients = patients;
            WorkspaceId = workspaceId;
            PatientId = patientId;
            Id = uploadProcessingResultEntity.Id;
            Uid = uploadProcessingResultEntity.Uid;
            Type = uploadProcessingResultEntity.Type;
            Modality = uploadProcessingResultEntity.Modality;
            Description = uploadProcessingResultEntity.Description;
        }

        /// <summary>
        /// Gets the complete representation of the entity
        /// </summary>
        /// <returns>A complete representation of the entity</returns>
        /// <example>This example shows how to get a list of entities associated with a given upload:
        /// <code>
        /// using ProKnow;
        /// using System.Collections.Generic;
        /// using System.Linq;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var uploadBatch = await pk.Uploads.UploadAsync("Upload Test", "./DICOM");
        /// var entityItems = new List&lt;EntityItem&gt;();
        /// foreach (var patient in uploadBatch.Patients)
        /// {
        ///     entityItems.AddRange(await Task.WhenAll(patient.Entities.Select(async s => await s.GetAsync())));
        /// }
        /// </code>
        /// </example>
        public async Task<EntityItem> GetAsync()
        {
            var patientItem = await _patients.GetAsync(WorkspaceId, PatientId);
            var entitySummary = patientItem.FindEntities(e => e.Id == Id)[0];
            return await entitySummary.GetAsync();
        }
    }
}
