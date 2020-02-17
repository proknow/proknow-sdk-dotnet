﻿using System;
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
            var patientMetadata = new PatientMetadata { Mrn = mrn, Name = name, BirthDate = birthDate, Sex = sex };
            var content = new StringContent(JsonSerializer.Serialize(patientMetadata), Encoding.UTF8, "application/json");
            var patientItemJson = await _proKnow.Requestor.PostAsync($"/workspaces/{workspaceItem.Id}/patients", null, content);
            return DeserializePatient(workspaceItem.Id, patientItemJson);
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
        public Task<PatientSummary> FindAsync(string workspace, Func<PatientSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            return QueryAsync(workspace).ContinueWith(patientSummariesTask =>
            {
                var patientSummaries = patientSummariesTask.Result;
                foreach (var patientSummary in patientSummaries)
                {
                    if (predicate(patientSummary))
                    {
                        return patientSummary;
                    }
                }
                return null;
            });
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
                var content = new StringContent(JsonSerializer.Serialize(mrns), Encoding.UTF8, "application/json");
                var patientsJsonTask = _proKnow.Requestor.PostAsync($"/workspaces/{workspaceId}/patients/lookup", null, content);
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
            //todo--use searchString
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
            patientItem.PostProcessDeserialization(_proKnow.Requestor, workspaceId);
            return patientItem;
        }
    }
}
