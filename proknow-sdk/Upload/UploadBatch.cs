using System;
using System.Collections.Generic;

namespace ProKnow.Upload
{
    /// <summary>
    /// Looks up patients and entities within a batch of resolved uploads
    /// </summary>
    public class UploadBatch
    {
        private ProKnow _proKnow;
        private string _workspaceId;
        private Dictionary<string, UploadStatusResult> _fileLookup;
        private Dictionary<string, UploadPatientSummary> _patientLookup;
        private Dictionary<string, UploadEntitySummary> _entityLookup;

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
        public UploadBatch(ProKnow proKnow, string workspaceId, IList<UploadStatusResult> uploadStatusResults)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _fileLookup = new Dictionary<string, UploadStatusResult>();
            _patientLookup = new Dictionary<string, UploadPatientSummary>();
            _entityLookup = new Dictionary<string, UploadEntitySummary>();
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

                    var entityId = uploadStatusResult.Entity.Id;
                    if (!_entityLookup.ContainsKey(entityId))
                    {
                        var uploadEntitySummary = new UploadEntitySummary(_proKnow.Patients, _workspaceId, patientId, uploadStatusResult.Entity);
                        _entityLookup[entityId] = uploadEntitySummary;
                        _patientLookup[patientId].Entities.Add(uploadEntitySummary);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the upload patient summary for a file
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The upload patient summary for the specified file</returns>
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
        /// Finds the upload entity summary for a file
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>The upload patient summary for the specified file</returns>
        public UploadEntitySummary FindEntity(string path)
        {
            if (_fileLookup.ContainsKey(path))
            {
                var uploadStatusResult = _fileLookup[path];
                if (uploadStatusResult.Status == "completed")
                {
                    return _entityLookup[uploadStatusResult.Entity.Id];
                }
                throw new ApplicationException($"The upload for '{path}' is not complete.");
            }
            throw new ApplicationException($"The upload for '{path}' was not found in the batch.");
        }
    }
}
