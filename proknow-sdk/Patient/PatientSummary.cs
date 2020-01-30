using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
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
        /// Determines whether this object satisfies a predicate and/or specified property values
        /// </summary>
        /// <param name="predicate">The optional predicate</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>True if this object satisfies the predicate (if specified) and all property filters (if specified); otherwise false</returns>
        public bool DoesMatch(Func<PatientSummary, bool> predicate = null, params KeyValuePair<string, object>[] properties)
        {
            if (predicate != null && !predicate(this))
            {
                return false;
            }
            foreach (var kvp in properties)
            {
                switch (kvp.Key)
                {
                    case "id":
                        if (!Id.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "mrn":
                        if (!Mrn.Equals(kvp.Value))
                        {
                           return false;
                        }
                        break;
                    case "name":
                        if (!Name.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "birth_date":
                        if (!BirthDate.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "sex":
                        if (!Sex.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    default:
                        if (!Data.ContainsKey(kvp.Key) || !Data[kvp.Key].Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Asynchronously gets the corresponding patient item
        /// </summary>
        /// <returns>The corresponding patient item</returns>
        public Task<PatientItem> GetAsync()
        {
            return Patients.GetAsync(WorkspaceId, Id);
        }

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
        /// <param name="patients">The parent Patients object</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal void PostProcessDeserialization(Patients patients, string workspaceId)
        {
            Patients = patients;
            WorkspaceId = workspaceId;
        }
    }
}
