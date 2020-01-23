using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Patient
{
    /// <summary>
    /// Interacts with patients in a ProKnow organization
    /// </summary>
    public class Patients
    {
        private ProKnow _proKnow;

        /// <summary>
        /// Constructs a patients object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        public Patients(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Asynchronously gets the specified patient
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace containing the patient</param>
        /// <param name="patientId">The ProKnow ID of the patient</param>
        /// <returns>The specified patient item or null if it was not found</returns>
        public Task<PatientItem> GetAsync(string workspaceId, string patientId)
        {
            var patientJsonTask = _proKnow.Requestor.getAsync($"/workspaces/{workspaceId}/patients/{patientId}");
            return patientJsonTask.ContinueWith(t => DeserializePatient(workspaceId, t.Result));
        }

        /// <summary>
        /// Queries the ProKnow API for the collection of patient summaries
        /// </summary>
        /// <param name="workspace">ID or name of the workspace containing the patients</param>
        /// <param name="searchString"></param>
        /// <returns>A collection of patient summaries</returns>
        public Task<IList<PatientSummary>> QueryAsync(string workspace, string searchString = null)
        {
            var workspaceIdTask = _proKnow.Workspaces.ResolveAsync(workspace);
            return workspaceIdTask.ContinueWith(t1 =>
                {
                    var workspaceId = t1.Result.Id;
                    var patientsJsonTask = _proKnow.Requestor.getAsync($"/workspaces/{workspaceId}/patients");
                    return patientsJsonTask.ContinueWith(t2 => DeserializePatients(workspaceId, t2.Result));
                }).Unwrap();
        }

        /// <summary>
        /// Creates a collection of patient summaries from their JSON representation
        /// </summary>
        /// <param name="workspaceId">ID of the workspace containing the patients</param>
        /// <param name="patientsJson">JSON representation of a collection of patient summaries</param>
        /// <returns>A collection of patient summaries</returns>
        private IList<PatientSummary> DeserializePatients(string workspaceId, string patientsJson)
        {
            var patientsData = JsonSerializer.Deserialize<IList<Dictionary<string, object>>>(patientsJson);
            return patientsData.Select(p => new PatientSummary(this, workspaceId, p)).ToList();
        }

        /// <summary>
        /// Creates a patient item from its JSON representation
        /// </summary>
        /// <param name="workspaceId">ID of the workspace containing the patients</param>
        /// <param name="patientItemJson">JSON representation of the patient item</param>
        /// <returns>A patient item</returns>
        private PatientItem DeserializePatient(string workspaceId, string patientItemJson)
        {
            var patientItem = JsonSerializer.Deserialize<PatientItem>(patientItemJson);
            patientItem.Patients = this;
            patientItem.WorkspaceId = workspaceId;

            // Add properties that were specifically de-serialized into collection of unspecified properties de-serialized
            patientItem.Data.Add("id", patientItem.Id);
            patientItem.Data.Add("mrn", patientItem.Mrn);
            patientItem.Data.Add("name", patientItem.Name);
            patientItem.Data.Add("birth_date", patientItem.BirthDate);
            patientItem.Data.Add("sex", patientItem.Sex);
            patientItem.Data.Add("metadata", patientItem.Metadata);

            //todo--Studies

            //todo--Tasks

            return patientItem;
        }
    }
}
