using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// The base class for entities in a ProKnow patient
    /// </summary>
    public abstract class EntityItem
    {
        /// <summary>
        /// Issues requests to the ProKnow API
        /// </summary>
        protected Requestor _requestor;

        /// <summary>
        /// The patient workspace ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonPropertyName("patient")]
        public string PatientId { get; set; }

        /// <summary>
        /// The entity ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The entity type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The entity series instance UID (image_set types) or SOP instance UID (other types)
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// The entity modality
        /// </summary>
        [JsonPropertyName("modality")]
        public string Modality { get; set; }

        /// <summary>
        /// The entity description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The entity metadata (custom metrics)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// The entity processing status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        //todo--Add Scorecards property

        /// <summary>
        /// Deletes this entity asynchronously
        /// </summary>
        public async Task DeleteAsync()
        {
            await _requestor.DeleteAsync($"/workspaces/{WorkspaceId}/entities/{Id}");
        }

        /// <summary>
        /// Downloads this entity asynchronously as DICOM object(s) to the specified folder
        /// </summary>
        /// <param name="root">The full path to the destination root folder</param>
        /// <returns>The full path to the destination folder (root or a sub-folder) to which the file(s) were downloaded</returns>
        public abstract Task<string> DownloadAsync(string root);

        //todo--Implement GetMetadataAsync method

        //todo--Implement SaveAsync method

        //todo--Implement SetMetadataAsync method

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return $"{Type} | {Uid}";
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="requestor">Issues requests to the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal virtual void PostProcessDeserialization(Requestor requestor, string workspaceId)
        {
            _requestor = requestor;
            WorkspaceId = workspaceId;
        }
    }
}
