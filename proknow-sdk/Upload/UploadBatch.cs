using ProKnow.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProKnow.Upload
{
    /// <summary>
    /// Looks up patients, entities, and spatial registration objects within a batch of resolved uploads
    /// </summary>
    public class UploadBatch
    {
        private readonly ProKnowApi _proKnow;
        private readonly string _workspaceId;
        private readonly Dictionary<string, UploadProcessingResult> _fileLookup; // by file path
        private readonly Dictionary<string, UploadPatientSummary> _patientLookup; // by patient ProKnow ID
        private readonly Dictionary<string, UploadEntitySummary> _entityLookup; // by entity ProKnow ID
        private readonly Dictionary<string, UploadSroSummary> _sroLookup; // by SRO ProKnow ID

        /// <summary>
        /// The collection of patient summaries
        /// </summary>
        public IList<UploadPatientSummary> Patients { get; protected set; }

        /// <summary>
        /// The modality and image count for each image set successfully processed
        /// </summary>
        public IList<Tuple<string, int>> ImageSetsModalityCount { get; protected set; }

        /// <summary>
        /// The number of structure sets successfully processed
        /// </summary>
        public int StructureSetCount { get; protected set; }

        /// <summary>
        /// The number of plans successfully processed
        /// </summary>
        public int PlanCount { get; protected set; }

        /// <summary>
        /// The number of doses successfully processed
        /// </summary>
        public int DoseCount { get; protected set; }

        /// <summary>
        /// The number of spatial registration objects successfully processed
        /// </summary>
        public int SroCount { get; protected set; }

        /// <summary>
        /// The number of files successfully processed
        /// </summary>
        public int CompletedCount { get; protected set; }

        /// <summary>
        ///  The number of files that are pending processing and need attention
        /// </summary>
        public int PendingCount { get; protected set; }

        /// <summary>
        /// The number of files that failed processing
        /// </summary>
        public int FailedCount { get; protected set; }

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
            Summarize();
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

        /// <summary>
        /// Summarize the upload batch
        /// </summary>
        private void Summarize()
        {
            // Map file paths to upload processing result entity/SRO
            var fileToUploadProcessingResultEntity = _fileLookup.Where(x => x.Value.Entity != null).ToDictionary(x => x.Key, x => x.Value.Entity);
            var fileToUploadProcessingResultSro = _fileLookup.Where(x => x.Value.Sro != null).ToDictionary(x => x.Key, x => x.Value.Sro);

            // Map ProKnow entity to number of files
            var entityToFileCount = fileToUploadProcessingResultEntity.GroupBy(x => x.Value.Id).ToDictionary(x => x.First().Value, x => x.Count());

            // Filter image set entities
            var imageSetsToFileCount = entityToFileCount.Where(x => x.Key.Type == "image_set").ToDictionary(x => x.Key, x => x.Value);

            // Initialize the properties that provide the summary
            ImageSetsModalityCount = imageSetsToFileCount.Select(x => new Tuple<string, int>(x.Key.Modality, x.Value)).ToList();
            StructureSetCount = entityToFileCount.Where(x => x.Key.Type == "structure_set").Count();
            PlanCount = entityToFileCount.Where(x => x.Key.Type == "plan").Count();
            DoseCount = entityToFileCount.Where(x => x.Key.Type == "dose").Count();
            SroCount = _sroLookup.Keys.Count;
            CompletedCount = _fileLookup.Where(x => x.Value.Status == "completed").Count();
            PendingCount = _fileLookup.Where(x => x.Value.Status == "pending").Count();
            FailedCount = _fileLookup.Where(x => x.Value.Status == "failed").Count();
        }
    }
}
