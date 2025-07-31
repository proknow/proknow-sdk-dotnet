using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Registrations
{
    /// <summary>
    /// Provides a summary of a spatial registration object (SRO) for a patient
    /// </summary>
    public class SroSummary
    {
        private const int RETRY_DELAY = 200;
        private const int MAX_TOTAL_RETRY_DELAY = 5000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

        private ProKnowApi _proKnow;

        /// <summary>
        /// The workspace ProKnow ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonIgnore]
        public string PatientId { get; internal set; }

        /// <summary>
        /// The study ProKnow ID
        /// </summary>
        [JsonPropertyName("study")]
        public string StudyId { get; set; }

        /// <summary>
        /// The SRO ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The SRO SOP instance UID
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// The SRO name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The SRO type, e.g., "rigid"
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The source frame of reference UID
        /// </summary>
        [JsonPropertyName("source_frame_of_reference")]
        public string SourceFrameOfReferenceUid { get; set; }

        /// <summary>
        /// The source image set ProKnow ID
        /// </summary>
        [JsonPropertyName("source_image_set_id")]
        public string SourceImageSetId { get; set; }

        /// <summary>
        /// The target frame of reference UID
        /// </summary>
        [JsonPropertyName("target_frame_of_reference")]
        public string TargetFrameOfReferenceUid { get; set; }

        /// <summary>
        /// The target image set ProKnow ID
        /// </summary>
        [JsonPropertyName("target_image_set_id")]
        public string TargetImageSetId { get; set; }

        /// <summary>
        /// The processing status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Deletes the corresponding SRO item asynchronously
        /// </summary>
        public Task DeleteAsync()
        {
           return _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/sros/{Id}");
        }

        /// <summary>
        /// Asynchronously gets the corresponding SRO item
        /// </summary>
        /// <returns>The corresponding SRO item</returns>
        public async Task<SroItem> GetAsync()
        {
            var numberOfRetries = 0;
            while (true)
            {
                var json = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/sros/{Id}");
                var sroItem = new SroItem(_proKnow, WorkspaceId, PatientId, json);
                if (sroItem.Status == "completed")
                {
                    return sroItem;
                }
                else
                {
                    if (numberOfRetries < MAX_RETRIES)
                    {
                        await Task.Delay(RETRY_DELAY);
                        numberOfRetries++;
                    }
                    else
                    {
                        throw new TimeoutException("Timeout while waiting for SRO to reach completed status.");
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Id;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId, string patientId)
        {
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
            PatientId = patientId;
        }
    }
}
