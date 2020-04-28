using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Represents a spatial registration object (SRO) for a patient
    /// </summary>
    public class SroItem
    {
        /// <summary>
        /// Root object for interfacing with the ProKnow API
        /// </summary>
        protected ProKnowApi _proKnow;

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
        /// The source for the registration
        /// </summary>
        [JsonPropertyName("source")]
        public RegistrationCoordinateSystem Source { get; set; }

        /// <summary>
        /// The target for the registration
        /// </summary>
        [JsonPropertyName("target")]
        public RegistrationCoordinateSystem Target { get; set; }

        /// <summary>
        /// The source-to-target transformation
        /// </summary>
        [JsonPropertyName("rigid")]
        public SroTransformation Transformation { get; set; }

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
        /// Used by deserialization to create an SRO item
        /// </summary>
        public SroItem()
        {
        }

        /// <summary>
        /// Creates an SRO item from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="patientId">The patient ProKnow ID</param>
        /// <param name="json">The JSON representation of the SRO item</param>
        internal SroItem(ProKnowApi proKnow, string workspaceId, string patientId, string json)
        {
            var sroItem = JsonSerializer.Deserialize<SroItem>(json);
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
            PatientId = patientId;
            StudyId = sroItem.StudyId;
            Id = sroItem.Id;
            Uid = sroItem.Uid;
            Name = sroItem.Name;
            Type = sroItem.Type;
            Source = sroItem.Source;
            Target = sroItem.Target;
            Transformation = sroItem.Transformation;
            Status = sroItem.Status;
            ExtensionData = sroItem.ExtensionData;
        }
    }
}
