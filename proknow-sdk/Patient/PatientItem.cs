using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Represents a patient in a ProKnow workspace
    /// </summary>
    public class PatientItem
    {
        private Requestor _requestor;

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
        /// All patient attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

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
        /// The patient metadata (custom metrics)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// Entities within this study
        /// </summary>
        [JsonPropertyName("studies")]
        public IList<StudySummary> Studies { get; set; }

        //todo--Tasks

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="requestor">Issues requests to the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal void PostProcessDeserialization(Requestor requestor, string workspaceId)
        {
            _requestor = requestor;
            WorkspaceId = workspaceId;

            // Post-process deserialization of studies
            foreach (var study in Studies)
            {
                study.PostProcessDeserialization(_requestor, WorkspaceId, Id);
            }

            //todo--Tasks
        }
    }
}
