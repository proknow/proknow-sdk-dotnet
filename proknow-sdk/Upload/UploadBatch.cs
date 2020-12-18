using ProKnow.Exceptions;
using System.Collections.Generic;

namespace ProKnow.Upload
{
    /// <summary>
    /// Looks up patients, entities, and spatial registration objects within a batch of resolved uploads
    /// </summary>
    public class UploadBatch
    {
        private readonly ProKnowApi _proKnow;
        private readonly string _workspaceId;
        private readonly Dictionary<string, UploadProcessingResult> _fileLookup;
        private readonly Dictionary<string, UploadPatientSummary> _patientLookup;
        private readonly Dictionary<string, UploadEntitySummary> _entityLookup;
        private readonly Dictionary<string, UploadSroSummary> _sroLookup;

        /// <summary>
        /// The collection of patient summaries
        /// </summary>
        public IList<UploadPatientSummary> Patients { get; protected set; }

        /// <summary>
        /// Creates a batch of resolved uploads
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The ProKnow ID of the workspace</param>
        /// <param name="uploadProcessingResults">The upload processing results</param>
        /// <example>This example shows how to create an upload batch:
        /// <code>
        /// using ProKnow;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// var uploadBatch = new UploadBatch(pk, workspaceItem.Id, uploadProcessingResults);
        /// </code>
        /// </example>
        public UploadBatch(ProKnowApi proKnow, string workspaceId, IList<UploadProcessingResult> uploadProcessingResults)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _fileLookup = new Dictionary<string, UploadProcessingResult>();
            _patientLookup = new Dictionary<string, UploadPatientSummary>();
            _entityLookup = new Dictionary<string, UploadEntitySummary>();
            _sroLookup = new Dictionary<string, UploadSroSummary>();
            Patients = new List<UploadPatientSummary>();
            foreach (var uploadProcessingResult in uploadProcessingResults)
            {
                _fileLookup[uploadProcessingResult.Path] = uploadProcessingResult;
                if (uploadProcessingResult.Status == "completed")
                {
                    var patientId = uploadProcessingResult.Patient.Id;
                    if (!_patientLookup.ContainsKey(patientId))
                    {
                        var uploadPatientSummary = new UploadPatientSummary(_proKnow.Patients, _workspaceId, uploadProcessingResult.Patient);
                        _patientLookup[patientId] = uploadPatientSummary;
                        Patients.Add(uploadPatientSummary);
                    }

                    if (uploadProcessingResult.Entity != null)
                    {
                        var entityId = uploadProcessingResult.Entity.Id;
                        if (!_entityLookup.ContainsKey(entityId))
                        {
                            var uploadEntitySummary = new UploadEntitySummary(_proKnow.Patients, _workspaceId, patientId, uploadProcessingResult.Entity);
                            _entityLookup[entityId] = uploadEntitySummary;
                            _patientLookup[patientId].Entities.Add(uploadEntitySummary);
                        }
                    }

                    if (uploadProcessingResult.Sro != null)
                    {
                        var sroId = uploadProcessingResult.Sro.Id;
                        if (!_sroLookup.ContainsKey(sroId))
                        {
                            var studyId = uploadProcessingResult.Study.Id;
                            var uploadSroSummary = new UploadSroSummary(_proKnow, _workspaceId, patientId, studyId, uploadProcessingResult.Sro);
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
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// var uploadBatch = new UploadBatch(pk, workspaceItem.Id, uploadProcessingResults);
        /// var path = Path.GetFullPath(Path.Join("./DICOM", "plan.dcm"));
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
                throw new InvalidOperationError($"The upload for '{path}' is not complete.");
            }
            throw new InvalidOperationError($"The upload for '{path}' was not found in the batch.");
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
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// var uploadBatch = new UploadBatch(pk, workspaceItem.Id, uploadProcessingResults);
        /// var path = Path.GetFullPath(Path.Join("./DICOM", "plan.dcm"));
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
                    throw new InvalidOperationError($"The uploaded file '{path}' is not an entity.");
                }
                throw new InvalidOperationError($"The upload for '{path}' is not complete.");
            }
            throw new InvalidOperationError($"The upload for '{path}' was not found in the batch.");
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
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// var uploadBatch = new UploadBatch(pk, workspaceItem.Id, uploadProcessingResults);
        /// var path = Path.GetFullPath(Path.Join("./DICOM", "reg.dcm"));
        /// var uploadSroSummary = uploadBatch.FindSro(path);
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
                    throw new InvalidOperationError($"The uploaded file '{path}' is not a spatial registration object.");
                }
                throw new InvalidOperationError($"The upload for '{path}' is not complete.");
            }
            throw new InvalidOperationError($"The upload for '{path}' was not found in the batch.");
        }

        /// <summary>
        /// Get the processing status for a provided path
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>"completed" if the upload processing was successful, "pending" if the upload was successful
        /// but conflicts occurred during processing and it needs attention, or "failed" if the upload could not be processed. </returns>
        /// <example>This example shows how to get the processing status for a given file upload:
        /// <code>
        /// using ProKnow;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// var uploadBatch = new UploadBatch(pk, workspaceItem.Id, uploadProcessingResults);
        /// var path = Path.GetFullPath(Path.Join("./DICOM", "reg.dcm"));
        /// var status = uploadBatch.GetStatus(path);
        /// </code>
        /// </example>
        public string GetStatus(string path)
        {
            if (_fileLookup.ContainsKey(path))
            {
                var uploadStatusResult = _fileLookup[path];
                return uploadStatusResult.Status;
            }
            throw new InvalidOperationError($"The upload for '{path}' was not found in the batch.");
        }
    }
}
