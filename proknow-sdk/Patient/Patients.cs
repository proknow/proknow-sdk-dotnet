using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ProKnow.Exceptions;
using ProKnow.Patient.Document;

namespace ProKnow.Patient
{
    /// <summary>
    /// Interacts with patients in a ProKnow organization
    /// </summary>
    public class Patients
    {
        private readonly ProKnowApi _proKnow;

        /// <summary>
        /// Interacts with documents for patients
        /// </summary>
        public Documents Documents { get; private set; }

        /// <summary>
        /// Constructs a patients object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Patients(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
            Documents = new Documents(_proKnow);
        }

        /// <summary>
        /// Creates a patient asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace in which to create the patient</param>
        /// <param name="mrn">The patient medical record number (DICOM ID)</param>
        /// <param name="name">The patient name</param>
        /// <param name="birthDate">The patient birth date in the format "YYYY-MM-DD" or null</param>
        /// <param name="sex">The patient sex, one of "M", "F", "O" or null</param>
        /// <returns>The created patient item</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <example>This example shows how to create a patient in the workspace called "Clinical":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var patient = await pk.Patients.CreateAsync("Clinical");
        /// </code>
        /// </example>
        public async Task<PatientItem> CreateAsync(string workspace, string mrn, string name, string birthDate = null, string sex = null)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var properties = new Dictionary<string, object>() { { "mrn", mrn }, { "name", name }, { "birth_date", birthDate }, { "sex", sex } };
            var requestContent = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync($"/workspaces/{workspaceItem.Id}/patients", null, requestContent);
            return new PatientItem(_proKnow, workspaceItem.Id, responseJson);
        }

        /// <summary>
        /// Deletes a patient asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the workspace containing the patient</param>
        /// <param name="patientId">The ProKnow ID of the patient to delete</param>
        /// <example>If you know the workspace ID and patient ID, you can delete a patient directly using this method:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// await pk.Patients.DeleteAsync("5c463a6c040040f1efda74db75c1b121", "5c4b4c52a5c058c3d1d98ac194d0200f");
        /// </code>
        /// </example>
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
        /// <remarks>
        /// For more information on how to use this method, see [Using Find Methods](../articles/usingFindMethods.md)
        /// </remarks>
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
        /// <example>If you know the workspace ID and patient ID, you can get the patient directly using this method:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// await pk.Patients.GetAsync("5c463a6c040040f1efda74db75c1b121", "5c4b4c52a5c058c3d1d98ac194d0200f");
        /// </code>
        /// </example>
        public async Task<PatientItem> GetAsync(string workspaceId, string patientId)
        {
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/patients/{patientId}");
            return new PatientItem(_proKnow, workspaceId, json);
        }

        /// <summary>
        /// Looks up a collection of patients matching a given list of MRNs
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace in which to lookup patients</param>
        /// <param name="mrns">The list of MRNs to look up</param>
        /// <returns>A collection of patient summaries.  If the MRN at a given index cannot be found, the result will contain will
        /// contain null at that index</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <example>Use this method to resolve a list of patient MRNs.  Just provide the MRNs as a list for the second argument:
        /// <code>
        /// using ProKnow;
        /// using System.Collections.Generic;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var patientSummaries = await pk.Patients.LookupAsync("Clinical", new List&lt;string&gt;() { "HNC-0522c0009", "HNC-0522c0013" });
        /// </code>
        /// </example>
        public async Task<IList<PatientSummary>> LookupAsync(string workspace, IList<string> mrns)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var workspaceId = workspaceItem.Id;
            var requestContent = new StringContent(JsonSerializer.Serialize(mrns), Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync($"/workspaces/{workspaceId}/patients/lookup", null, requestContent);
            return DeserializePatients(workspaceId, responseJson);
        }

        /// <summary>
        /// Queries the ProKnow API for the collection of patient summaries
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace containing the patients</param>
        /// <param name="searchString">If provided, returns only the patients whose MRN or name match the parameter</param>
        /// <returns>A collection of patient summaries</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <example>This example queries the patients belonging to the Clinical workspace and prints the name of each patient:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var patientSummaries = await pk.Patients.QueryAsync("Clinical");
        /// foreach (var patientSummary in patientSummaries)
        /// {
        ///     Console.WriteLine(patientSummary.Name);
        /// }
        /// </code>
        /// </example>
        public async Task<IList<PatientSummary>> QueryAsync(string workspace, string searchString = null)
        {
            //todo--paging (response header includes "proknow-has-more" -> "true")
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var workspaceId = workspaceItem.Id;
            Dictionary<string, object> queryParameters = null;
            if (searchString != null)
            {
                queryParameters = new Dictionary<string, object>() { { "search", searchString } };
            }
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/patients", null, queryParameters);
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
