using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Patient
{
    /// <summary>
    /// Interacts with documents for patients in a ProKnow organization
    /// </summary>
    public class Documents
    {
        private ProKnow _proKnow;

        /// <summary>
        /// Constructs a documents object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        public Documents(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a patient document
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        /// <param name="path">The full path to the document</param>
        public async Task CreateAsync(string workspaceId, string patientId, string path)
        {
            var documentName = Path.GetFileName(path);
            var route = $"/workspaces/{workspaceId}/patients/{patientId}/documents/{documentName}";
            using (var content = new MultipartFormDataContent())
            {
                using (var fs = File.OpenRead(path))
                {
                    content.Add(new StreamContent(fs), "documentName", path);
                    await _proKnow.Requestor.PostAsync(route, null, content);
                }
            }
        }

        // stream

        // query

        // update

        // delete
    }
}