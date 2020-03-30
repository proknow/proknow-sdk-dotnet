using ProKnow.Patient.Entities;
using ProKnow.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient
{
    /// <summary>
    /// Represents a patient in a ProKnow workspace
    /// </summary>
    public class PatientItem
    {
        private ProKnow _proKnow;

        /// <summary>
        /// The patient workspace ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The patient medical record number (MRN) or ID
        /// </summary>
        [JsonPropertyName("mrn")]
        public string Mrn { get; set; }

        /// <summary>
        /// The patient name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The patient birth date
        /// </summary>
        [JsonPropertyName("birth_date")]
        public string BirthDate { get; set; }

        /// <summary>
        /// The patient sex
        /// </summary>
        [JsonPropertyName("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// The patient metadata (custom metrics) referenced by ProKnow ID
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Entities within this study
        /// </summary>
        [JsonPropertyName("studies")]
        public IList<StudySummary> Studies { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        //todo--Add Tasks property

        /// <summary>
        /// Used by deserialization to create patient item
        /// </summary>
        public PatientItem()
        {
        }

        /// <summary>
        /// Creates a patient item from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">ID of the workspace containing the patients</param>
        /// <param name="json">JSON representation of the patient item</param>
        internal PatientItem(ProKnow proKnow, string workspaceId, string json)
        {
            Initialize(proKnow, workspaceId, json);
        }

        /// <summary>
        /// Deletes this patient item asynchronously
        /// </summary>
        public Task DeleteAsync()
        {
            return _proKnow.Patients.DeleteAsync(WorkspaceId, Id);
        }

        /// <summary>
        /// Finds the entities for this patient that satisfy a predicate
        /// </summary>
        /// <param name="predicate">A predicate for the search</param>
        /// <returns>All of the patient entities that satisfy the predicate or null if the predicate was null or no patient
        /// entity satisfies the predicate</returns>
        public IList<EntitySummary> FindEntities(Func<EntitySummary, bool> predicate)
        {
            IList<EntitySummary> matchingEntities = new List<EntitySummary>();
            if (predicate == null)
            {
                return matchingEntities;
            }
            foreach (var study in Studies)
            {
                foreach (var entityI in study.Entities)
                {
                    if (predicate(entityI))
                    {
                        matchingEntities.Add(entityI);
                    }
                    foreach (var entityJ in entityI.Entities)
                    {
                        if (predicate(entityJ))
                        {
                            matchingEntities.Add(entityJ);
                        }
                        foreach (var entityK in entityJ.Entities)
                        {
                            if (predicate(entityK))
                            {
                                matchingEntities.Add(entityK);
                            }
                            foreach (var entityM in entityK.Entities)
                            {
                                if (predicate(entityM))
                                {
                                    matchingEntities.Add(entityM);
                                }
                            }
                        }
                    }
                }
            }
            return matchingEntities;
        }

        /// <summary>
        /// Asynchronously resolves the metadata to a dictionary of custom metric names and values
        /// </summary>
        /// <returns>A dictionary of custom metric names and values</returns>
        public async Task<IDictionary<string, object>> GetMetadataAsync()
        {
            var customMetricItems = await Task.WhenAll(Metadata.Keys.Select(async (k) =>
                await _proKnow.CustomMetrics.ResolveByIdAsync(k)));
            var metadata = new Dictionary<string, object>();
            foreach (var customMetricItem in customMetricItems)
            {
                metadata.Add(customMetricItem.Name, Metadata[customMetricItem.Id]);
            }
            return metadata;
        }

        /// <summary>
        /// Refreshes the state of the patient
        /// </summary>
        public async Task RefreshAsync()
        {
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/patients/{Id}");
            Initialize(_proKnow, WorkspaceId, json);
        }

        /// <summary>
        /// Saves changes to a patient asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            var patientSchema = new PatientSaveSchema { Mrn = Mrn, Name = Name, BirthDate = BirthDate, Sex = Sex,
                Metadata = Metadata };
            var content = new StringContent(JsonSerializer.Serialize(patientSchema), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/workspaces/{WorkspaceId}/patients/{Id}", null, content);
        }

        /// <summary>
        /// Sets the metadata to an encoded version of the provided metadata
        /// </summary>
        /// <param name="metadata">A dictionary of custom metric names and values</param>
        public async Task SetMetadataAsync(IDictionary<string, object> metadata)
        {
            if (metadata == null)
            {
                Metadata = null;
            }
            else
            {
                var customMetricItems = await Task.WhenAll(metadata.Keys.Select(async (k) =>
                    await _proKnow.CustomMetrics.ResolveByNameAsync(k)));
                var resolvedMetadata = new Dictionary<string, object>();
                foreach (var customMetricItem in customMetricItems)
                {
                    resolvedMetadata.Add(customMetricItem.Id, metadata[customMetricItem.Name]);
                }
                Metadata = resolvedMetadata;
            }
        }

        /// <summary>
        /// Upload file(s) asynchronously
        /// </summary>
        /// <param name="path">The folder or file path</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <returns>The upload results</returns>
        public Task<UploadBatch> UploadAsync(string path, UploadFileOverrides overrides = null)
        {
            return _proKnow.Uploads.UploadAsync(WorkspaceId, path, overrides);
        }

        /// <summary>
        /// Upload files asynchronously
        /// </summary>
        /// <param name="paths">The folder and/or file paths</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <returns>The upload results</returns>
        public Task<UploadBatch> UploadAsync(IList<string> paths, UploadFileOverrides overrides = null)
        {
            return _proKnow.Uploads.UploadAsync(WorkspaceId, paths, overrides);
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return $"{Mrn} | {Name}";
        }

        /// <summary>
        /// Initializes this instance using its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">ID of the workspace containing the patients</param>
        /// <param name="json">JSON representation of the patient item</param>
        private void Initialize(ProKnow proKnow, string workspaceId, string json)
        {
            var patientItem = JsonSerializer.Deserialize<PatientItem>(json);
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
            Id = patientItem.Id;
            Mrn = patientItem.Mrn;
            Name = patientItem.Name;
            BirthDate = patientItem.BirthDate;
            Sex = patientItem.Sex;
            var metadata = new Dictionary<string, object>();
            foreach (var key in patientItem.Metadata.Keys)
            {
                var element = (JsonElement)patientItem.Metadata[key];
                switch (element.ValueKind)
                {
                    case JsonValueKind.String:
                        metadata[key] = element.GetString();
                        break;
                    case JsonValueKind.Number:
                        var number = element.GetDouble();
                        if (number % 1 == 0)
                        {
                            metadata[key] = (int)number;
                        }
                        else
                        {
                            metadata[key] = number;
                        }
                        break;
                    default:
                        throw new JsonException($"{element.ValueKind} is not a supported type for custom metrics.");
                }
            }
            Metadata = metadata;
            foreach (var study in patientItem.Studies)
            {
                study.PostProcessDeserialization(_proKnow, WorkspaceId, Id);
            }
            Studies = patientItem.Studies;
            ExtensionData = patientItem.ExtensionData;
            //todo--Tasks
        }
    }
}
