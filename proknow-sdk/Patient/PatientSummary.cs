using ProKnow.Upload;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient
{
    /// <summary>
    /// Provides a summary of a patient in a ProKnow workspace
    /// </summary>
    public class PatientSummary
    {
        private ProKnowApi _proKnow;

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
        /// The patient birth date in the format "YYYY-MM-DD" or null
        /// </summary>
        [JsonPropertyName("birth_date")]
        public string BirthDate { get; set; }

        /// <summary>
        /// The patient sex, one of "M", "F", "O" or null
        /// </summary>
        [JsonPropertyName("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Asynchronously gets the corresponding patient item
        /// </summary>
        /// <returns>The corresponding patient item</returns>
        public Task<PatientItem> GetAsync()
        {
            return _proKnow.Patients.GetAsync(WorkspaceId, Id);
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
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId)
        {
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
        }
    }
}
