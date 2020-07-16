﻿using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Patient.Document
{
    /// <summary>
    /// Interacts with documents for patients in a ProKnow organization
    /// </summary>
    public class Documents
    {
        private readonly ProKnowApi _proKnow;

        /// <summary>
        /// Constructs a documents object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Documents(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a patient document asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <param name="path">The full path to the document</param>
        /// <param name="documentName">Optional name for document, including file extension</param>
        public async Task CreateAsync(string workspaceId, string patientId, string path, string documentName = null)
        {
            var documentLabel = documentName ?? Path.GetFileName(path);
            var route = $"/workspaces/{workspaceId}/patients/{patientId}/documents/{documentLabel}";
            using (var content = new MultipartFormDataContent())
            {
                using (var fs = File.OpenRead(path))
                {
                    content.Add(new StreamContent(fs), documentLabel, path);
                    await _proKnow.Requestor.PostAsync(route, null, content);
                }
            }
        }

        /// <summary>
        /// Deletes a patient document asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <param name="documentId">The ProKnow ID for the document</param>
        public async Task DeleteAsync(string workspaceId, string patientId, string documentId)
        {
            var route = $"/workspaces/{workspaceId}/patients/{patientId}/documents/{documentId}";
            await _proKnow.Requestor.DeleteAsync(route);
        }

        /// <summary>
        /// Queries patient documents asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <returns>Summaries of the patient documents</returns>
        public async Task<IList<DocumentSummary>> QueryAsync(string workspaceId, string patientId)
        {
            var route = $"/workspaces/{workspaceId}/patients/{patientId}/documents";
            var json = await _proKnow.Requestor.GetAsync(route);
            return JsonSerializer.Deserialize<IList<DocumentSummary>>(json);
        }

        /// <summary>
        /// Streams a patient document asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <param name="documentId">The ProKnow ID for the document</param>
        /// <param name="path">The full path to which to stream the document</param>
        /// <returns>The full path to the streamed document</returns>
        public async Task<string> StreamAsync(string workspaceId, string patientId, string documentId, string path)
        {
            var documentName = Path.GetFileName(path);
            var route = $"/workspaces/{workspaceId}/patients/{patientId}/documents/{documentId}/{documentName}";
            return await _proKnow.Requestor.StreamAsync(route, path);
        }

        /// <summary>
        /// Updates a patient document asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <param name="documentId">The ProKnow ID for the document</param>
        /// <param name="documentName">The updated document name</param>
        /// <param name="documentCategory">The updated document category</param>
        public async Task UpdateAsync(string workspaceId, string patientId, string documentId,
            string documentName, string documentCategory)
        {
            var route = $"/workspaces/{workspaceId}/patients/{patientId}/documents/{documentId}";
            var documentSchema = new DocumentUpdateSchema() { Name = documentName, Category = documentCategory };
            var content = new StringContent(JsonSerializer.Serialize(documentSchema), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync(route, null, content);
        }
    }
}
