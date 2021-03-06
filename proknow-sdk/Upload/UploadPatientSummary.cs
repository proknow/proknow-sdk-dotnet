﻿using ProKnow.Patient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProKnow.Upload
{
    /// <summary>
    /// A summary view of a patient in an upload response
    /// </summary>
    public class UploadPatientSummary
    {
        private readonly Patients _patients;

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
        /// Spatial registration objects within this patient
        /// </summary>
        public IList<UploadSroSummary> Sros { get; set; }

        /// <summary>
        /// Used by de-serialization to construct an UploadPatientSummary
        /// </summary>
        public UploadPatientSummary()
        {
        }

        /// <summary>
        /// Creates an upload patient summary
        /// </summary>
        /// <param name="patients">Interacts with patients in a ProKnow organization</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="uploadProcessingResultPatient">The patient data of an upload processing result</param>
        internal UploadPatientSummary(Patients patients, string workspaceId, UploadProcessingResultPatient uploadProcessingResultPatient)
        {
            _patients = patients;
            WorkspaceId = workspaceId;
            Entities = new List<UploadEntitySummary>();
            Sros = new List<UploadSroSummary>();
            Id = uploadProcessingResultPatient.Id;
            Mrn = uploadProcessingResultPatient.Mrn;
            Name = uploadProcessingResultPatient.Name;
        }

        /// <summary>
        /// Gets the complete representation of the patient
        /// </summary>
        /// <returns>The complete representation of the patient</returns>
        /// <example>This example shows how to get a list of patients associated with a given upload:
        /// <code>
        /// using ProKnow;
        /// using System.Linq;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// var uploadBatch = new UploadBatch(pk, workspaceItem.Id, uploadProcessingResults);
        /// var patientItems = await Task.WhenAll(uploadBatch.Patients.Select(async p => await p.GetAsync()));
        /// </code>
        /// </example>
        public Task<PatientItem> GetAsync()
        {
            return _patients.GetAsync(WorkspaceId, Id);
        }
    }
}
