using System;
using System.Collections.Generic;

namespace ProKnow.Upload
{
    /// <summary>
    /// Looks up patients, entities, and spatial registration objects within a batch of resolved uploads
    /// </summary>
    public class UploadBatch
    {
        private ProKnowApi _proKnow;
        private string _workspaceId;
        private Dictionary<string, UploadStatusResult> _fileLookup;
        private Dictionary<string, UploadPatientSummary> _patientLookup;
        private Dictionary<string, UploadEntitySummary> _entityLookup;
        private Dictionary<string, UploadSroSummary> _sroLookup;

        /// <summary>
        /// The collection of patient summaries
        /// </summary>
        public IList<UploadPatientSummary> Patients { get; protected set; }

        /// <summary>
        /// Creates a batch of resolved uploads
        /// </summary>
        /// <param name="proKnow"></param>
        /// <param name="workspaceId"></param>
        /// <param name="uploadStatusResults"></param>
        public UploadBatch(ProKnowApi proKnow, string workspaceId, IList<UploadStatusResult> uploadStatusResults)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _fileLookup = new Dictionary<string, UploadStatusResult>();
            _patientLookup = new Dictionary<string, UploadPatientSummary>();
            _entityLookup = new Dictionary<string, UploadEntitySummary>();
            _sroLookup = new Dictionary<string, UploadSroSummary>();
            Patients = new List<UploadPatientSummary>();
            foreach (var uploadStatusResult in uploadStatusResults)
            {
                _fileLookup[uploadStatusResult.Path] = uploadStatusResult;
                if (uploadStatusResult.Status == "completed")
                {
                    var patientId = uploadStatusResult.Patient.Id;
                    if (!_patientLookup.ContainsKey(patientId))
                    {
                        var uploadPatientSummary = new UploadPatientSummary(_proKnow.Patients, _workspaceId, uploadStatusResult.Patient);
                        _patientLookup[patientId] = uploadPatientSummary;
                        Patients.Add(uploadPatientSummary);
                    }

                    if (uploadStatusResult.Entity != null)
                    {
                        var entityId = uploadStatusResult.Entity.Id;
                        if (!_entityLookup.ContainsKey(entityId))
                        {
                            var uploadEntitySummary = new UploadEntitySummary(_proKnow.Patients, _workspaceId, patientId, uploadStatusResult.Entity);
                            _entityLookup[entityId] = uploadEntitySummary;
                            _patientLookup[patientId].Entities.Add(uploadEntitySummary);
                        }
                    }

                    if (uploadStatusResult.Sro != null)
                    {
                        var sroId = uploadStatusResult.Sro.Id;
                        if (!_sroLookup.ContainsKey(sroId))
                        {
                            var studyId = uploadStatusResult.Study.Id;
                            var uploadSroSummary = new UploadSroSummary(_proKnow, _workspaceId, patientId, studyId, uploadStatusResult.Sro);
                            _sroLookup[sroId] = uploadSroSummary;
                            _patientLookup[patientId].Sros.Add(uploadSroSummary);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the summary view of a patient for an uploaded file
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The summary view of the patient for an uploaded file</returns>
        /// <example>This example shows how to find a patient associated with a given file upload:
        /// <code>
        /// using ProKnow;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var uploadBatch = await pk.Uploads.UploadAsync("Upload Test", "DICOM");
        /// var path = Path.GetFullPath(Path.Join("DICOM", "plan.dcm"));
        /// var uploadPatientSummary = uploadBatch.FindPatient(path);
        /// </code>
        /// </example>
        public UploadPatientSummary FindPatient(string path)
        {
            if (_fileLookup.ContainsKey(path))
            {
                var uploadStatusResult = _fileLookup[path];
                if (uploadStatusResult.Status == "completed")
                {
                    return _patientLookup[uploadStatusResult.Patient.Id];
                }
                throw new ApplicationException($"The upload for '{path}' is not complete.");
            }
            throw new ApplicationException($"The upload for '{path}' was not found in the batch.");
        }

        /// <summary>
        /// Finds the summary view of an entity for an uploaded file
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The summary view of the entity for an uploaded file</returns>
        /// <example>This example shows how to find an entity associated with a given file upload:
        /// <code>
        /// using ProKnow;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var uploadBatch = await pk.Uploads.UploadAsync("Upload Test", "DICOM");
        /// var path = Path.GetFullPath(Path.Join("DICOM", "plan.dcm"));
        /// var uploadEntitySummary = uploadBatch.FindEntity(path);
        /// </code>
        /// </example>
        public UploadEntitySummary FindEntity(string path)
        {
            if (_fileLookup.ContainsKey(path))
            {
                var uploadStatusResult = _fileLookup[path];
                if (uploadStatusResult.Status == "completed")
                {
                    if (uploadStatusResult.Entity != null)
                    {
                        return _entityLookup[uploadStatusResult.Entity.Id];
                    }
                    throw new ApplicationException($"The uploaded file '{path}' is not an entity.");
                }
                throw new ApplicationException($"The upload for '{path}' is not complete.");
            }
            throw new ApplicationException($"The upload for '{path}' was not found in the batch.");
        }

        /// <summary>
        /// Finds the summary view of a spatial registration object for an uploaded file
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The summary view of the spatial registration object for an uploaded file</returns>
        /// <example>This example shows how to find a spatial registration object associated with a given file upload:
        /// <code>
        /// using ProKnow;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var uploadBatch = await pk.Uploads.UploadAsync("Upload Test", "./DICOM");
        /// var path = Path.GetFullPath(Path.Join("DICOM", "reg.dcm"));
        /// var uploadSroSummary = uploadBatch.Find(path);
        /// </code>
        /// </example>
        public UploadSroSummary FindSro(string path)
        {
            if (_fileLookup.ContainsKey(path))
            {
                var uploadStatusResult = _fileLookup[path];
                if (uploadStatusResult.Status == "completed")
                {
                    if (uploadStatusResult.Sro != null)
                    {
                        return _sroLookup[uploadStatusResult.Sro.Id];
                    }
                    throw new ApplicationException($"The uploaded file '{path}' is not a spatial registration object.");
                }
                throw new ApplicationException($"The upload for '{path}' is not complete.");
            }
            throw new ApplicationException($"The upload for '{path}' was not found in the batch.");
        }
    }
}
