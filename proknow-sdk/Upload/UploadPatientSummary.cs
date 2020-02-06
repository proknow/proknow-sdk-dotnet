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
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The patient medical record number (MRN) or ID
        /// </summary>
        public string Mrn { get; set; }

        /// <summary>
        /// The patient name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Entities within this patient
        /// </summary>
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
        /// Creates an upload patient summary
        /// </summary>
        /// <param name="patients">Interacts with patients in a ProKnow organization</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="uploadStatusResult">The upload status result</param>
        public UploadPatientSummary(Patients patients, string workspaceId, UploadStatusResultPatient uploadStatusResult)
        {
            _patients = patients;
            WorkspaceId = workspaceId;
            Entities = new List<UploadEntitySummary>();
            Id = uploadStatusResult.Id;
            Mrn = uploadStatusResult.Mrn;
            Name = uploadStatusResult.Name;
        }
    }
}
