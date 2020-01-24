using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Represents a patient in a ProKnow workspace
    /// </summary>
    public class PatientItem
    {
        /// <summary>
        /// The parent Patients object
        /// </summary>
        [JsonIgnore]
        internal Patients Patients { get; set; }

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
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="patients">The parent Patients object</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal void PostProcessDeserialization(Patients patients, string workspaceId)
        {
            Patients = patients;
            WorkspaceId = workspaceId;

            // Post-process deserialization of studies
            foreach (var study in Studies)
            {
                study.PostProcessDeserialization(patients, workspaceId, Id);
            }

            //todo--Tasks

            // Add member properties to collection of deserialized properties that had no matching member
            Data.Add("id", Id);
            Data.Add("mrn", Mrn);
            Data.Add("name", Name);
            Data.Add("birth_date", BirthDate);
            Data.Add("sex", Sex);
            Data.Add("metadata", Metadata);
            Data.Add("studies", Studies);
        }
    }
}
