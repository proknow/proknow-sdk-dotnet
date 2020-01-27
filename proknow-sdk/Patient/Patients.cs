using System.Collections.Generic;
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
            var patientJsonTask = _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/patients/{patientId}");
            return patientJsonTask.ContinueWith(t => DeserializePatient(workspaceId, t.Result));
        }

        /// <summary>
        /// Looks up a collection of patients matching a given list of MRNs
        /// </summary>
        /// <param name="workspace">The workspace ProKnow ID</param>
        /// <param name="mrns">The list of MRNs to look up</param>
        /// <returns>A collection of patient summaries.  If the MRN at a given index cannot be found, the result will contain will
        /// contain null at that index</returns>
        public Task<IList<PatientSummary>> LookupAsync(string workspace, IList<string> mrns)
        {
            var workspaceItemTask = _proKnow.Workspaces.ResolveAsync(workspace);
            return workspaceItemTask.ContinueWith(t1 =>
            {
                var workspaceId = t1.Result.Id;
                var patientsJsonTask = _proKnow.Requestor.PostAsync($"/workspaces/{workspaceId}/patients/lookup", mrns);
                return patientsJsonTask.ContinueWith(t2 => DeserializePatients(workspaceId, t2.Result));
            }).Unwrap();
        }

        /// <summary>
        /// Queries the ProKnow API for the collection of patient summaries
        /// </summary>
        /// <param name="workspace">ID or name of the workspace containing the patients</param>
        /// <param name="searchString"></param>
        /// <returns>A collection of patient summaries</returns>
        public Task<IList<PatientSummary>> QueryAsync(string workspace, string searchString = null)
        {
            var workspaceItemTask = _proKnow.Workspaces.ResolveAsync(workspace);
            return workspaceItemTask.ContinueWith(t1 =>
                {
                    var workspaceId = t1.Result.Id;
                    var patientsJsonTask = _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/patients");
                    return patientsJsonTask.ContinueWith(t2 => DeserializePatients(workspaceId, t2.Result));
                }).Unwrap();
        }

        /// <summary>
        /// Creates a collection of patient summaries from their JSON representation
        /// </summary>
        /// <param name="workspaceId">ID of the workspace containing the patients</param>
        /// <param name="json">JSON representation of a collection of patient summaries</param>
        /// <returns>A collection of patient summaries</returns>
        private IList<PatientSummary> DeserializePatients(string workspaceId, string json)
        {
            var patientSummaries = JsonSerializer.Deserialize<IList<PatientSummary>>(json);
            foreach (var patientSummary in patientSummaries)
            {
                if (patientSummary != null)
                {
                    patientSummary.PostProcessDeserialization(this, workspaceId);
                }
            }
            return patientSummaries;
        }

        /// <summary>
        /// Creates a patient item from its JSON representation
        /// </summary>
        /// <param name="workspaceId">ID of the workspace containing the patients</param>
        /// <param name="json">JSON representation of the patient item</param>
        /// <returns>A patient item</returns>
        private PatientItem DeserializePatient(string workspaceId, string json)
        {
            var patientItem = JsonSerializer.Deserialize<PatientItem>(json);
            patientItem.PostProcessDeserialization(this, workspaceId);
            return patientItem;
        }
    }
}
