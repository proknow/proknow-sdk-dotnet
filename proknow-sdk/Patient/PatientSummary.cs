using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ProKnow.Patient
{
    /// <summary>
    /// Provides a summary of a patient in a ProKnow workspace
    /// </summary>
    public class PatientSummary
    {
        /// <summary>
        /// The parent Patients object
        /// </summary>
        [JsonIgnore]
        internal Patients Patients { get; set; }

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
        /// All patient summary attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Asynchronously gets the corresponding patient item
        /// </summary>
        /// <returns>The corresponding patient item</returns>
        public Task<PatientItem> GetAsync()
        {
            return Patients.GetAsync(WorkspaceId, Id);
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="patients">The parent Patients object</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal void PostProcessDeserialization(Patients patients, string workspaceId)
        {
            Patients = patients;
            WorkspaceId = workspaceId;

            // Add member properties to collection of deserialized properties that had no matching member
            Data.Add("id", Id);
            Data.Add("mrn", Mrn);
            Data.Add("name", Name);
            Data.Add("birth_date", BirthDate);
            Data.Add("sex", Sex);
        }
    }
}
