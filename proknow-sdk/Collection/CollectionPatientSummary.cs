using ProKnow.Patient;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Collection
{
    /// <summary>
    /// Provides a summary of a patient in a collection
    /// </summary>
    public class CollectionPatientSummary
    {
        private ProKnowApi _proKnow;
        /// <summary>
        /// The workspace properties
        /// </summary>
        [JsonPropertyName("workspace")]
        public CollectionPatientSummaryWorkspace Workspace { get; set; }

        /// <summary>
        /// The patient properties
        /// </summary>
        [JsonPropertyName("patient")]
        public CollectionPatientSummaryPatient Patient { get; set; }

        /// <summary>
        /// The entity properties
        /// </summary>
        [JsonPropertyName("entity")]
        public CollectionPatientSummaryEntity Entity { get; set; }

        /// <summary>
        /// Gets the full representation of the patient
        /// </summary>
        /// <returns>A full representation of the patient</returns>
        public Task<PatientItem> GetAsync()
        {
            return _proKnow.Patients.GetAsync(Workspace.Id, Patient.Id);
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }
    }
}
