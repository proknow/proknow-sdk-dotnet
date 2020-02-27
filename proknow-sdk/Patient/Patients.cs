using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
        /// Creates a patient asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace in which to create the patient</param>
        /// <param name="mrn">The patient medical record number (DICOM ID)</param>
        /// <param name="name">The patient name</param>
        /// <param name="birthDate">The patient birth date</param>
        /// <param name="sex">The patient sex</param>
        /// <returns>The created patient item</returns>
        public async Task<PatientItem> CreateAsync(string workspace, string mrn, string name, string birthDate = null, string sex = null)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var patientSchema = new PatientCreateSchema { Mrn = mrn, Name = name, BirthDate = birthDate, Sex = sex };
            var requestContent = new StringContent(JsonSerializer.Serialize(patientSchema), Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync($"/workspaces/{workspaceItem.Id}/patients", null, requestContent);
            return new PatientItem(_proKnow, workspaceItem.Id, responseJson);
        }

        /// <summary>
        /// Deletes a patient asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the workspace containing the patient</param>
        /// <param name="patientId">The ProKnow ID of the patient to delete</param>
        public Task DeleteAsync(string workspaceId, string patientId)
        {
            return _proKnow.Requestor.DeleteAsync($"/workspaces/{workspaceId}/patients/{patientId}");
        }

        /// <summary>
        /// Finds a patient in a workspace asynchronously based on a predicate
        /// </summary>
        /// <param name="workspace">ID or name of the workspace containing the patients</param>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first patient that satisfies the predicate or null if the predicate was null or no patient satisfies
        /// the predicate</returns>
        public async Task<PatientSummary> FindAsync(string workspace, Func<PatientSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            var patientSummaries = await QueryAsync(workspace);
            foreach (var patientSummary in patientSummaries)
            {
                if (predicate(patientSummary))
                {
                    return patientSummary;
                }
            }
            return null;
        }

        /// <summary>
        /// Asynchronously gets the specified patient
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace containing the patient</param>
        /// <param name="patientId">The ProKnow ID of the patient</param>
        /// <returns>The specified patient item or null if it was not found</returns>
        public async Task<PatientItem> GetAsync(string workspaceId, string patientId)
        {
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/patients/{patientId}");
            return new PatientItem(_proKnow, workspaceId, json);
        }

        /// <summary>
        /// Looks up a collection of patients matching a given list of MRNs
        /// </summary>
        /// <param name="workspace">The workspace ProKnow ID</param>
        /// <param name="mrns">The list of MRNs to look up</param>
        /// <returns>A collection of patient summaries.  If the MRN at a given index cannot be found, the result will contain will
        /// contain null at that index</returns>
        public async Task<IList<PatientSummary>> LookupAsync(string workspace, IList<string> mrns)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var workspaceId = workspaceItem.Id;
            var requestContent = new StringContent(JsonSerializer.Serialize(mrns), Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync($"/workspaces/{workspaceId}/patients/lookup", null, requestContent);
            return DeserializePatients(workspaceId, responseJson);
        }

        //todo--Implement LookupAsync method

        /// <summary>
        /// Queries the ProKnow API for the collection of patient summaries
        /// </summary>
        /// <param name="workspace">ID or name of the workspace containing the patients</param>
        /// <param name="searchString"></param>
        /// <returns>A collection of patient summaries</returns>
        public async Task<IList<PatientSummary>> QueryAsync(string workspace, string searchString = null)
        {
            //todo--use searchString
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var workspaceId = workspaceItem.Id;
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/patients");
            return DeserializePatients(workspaceId, json);
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
                    patientSummary.PostProcessDeserialization(_proKnow, workspaceId);
                }
            }
            return patientSummaries;
        }
    }
}
