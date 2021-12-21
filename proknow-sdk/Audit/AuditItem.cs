using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Audit
{
    /// <summary>
    /// Represents a audit log item
    /// </summary>
    public class AuditItem
    {
        private ProKnowApi _proKnow;

        /// <summary>
        /// The ID of the log entry
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The user ID of the log entry
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// The user name of the log entry
        /// </summary>
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// The patient ID of the log entry
        /// </summary>
        [JsonPropertyName("patient_id")]
        public string PatientId { get; set; }

        /// <summary>
        /// The patient name of the log entry
        /// </summary>
        [JsonPropertyName("patient_name")]
        public string PatientName { get; set; }

        /// <summary>
        /// The patient MRN of the log entry
        /// </summary>
        [JsonPropertyName("patient_mrn")]
        public string PatientMrn { get; set; }

        /// <summary>
        /// The resource ID of the log entry
        /// </summary>
        [JsonPropertyName("resource_id")]
        public string ResourceId { get; set; }

        /// <summary>
        /// The resource name of the log entry
        /// </summary>
        [JsonPropertyName("resource_name")]
        public string ResourceName { get; set; }

        /// <summary>
        /// The workspace ID of the log entry
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public string WorkspaceId { get; set; }

        /// <summary>
        /// The workspace name of the log entry
        /// </summary>
        [JsonPropertyName("workspace_name")]
        public string WorkspaceName { get; set; }

        /// <summary>
        /// The collection ID of the log entry
        /// </summary>
        [JsonPropertyName("collection_id")]
        public string CollectionId { get; set; }

        /// <summary>
        /// The type of log entry
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The classification of the log entry
        /// </summary>
        [JsonPropertyName("classification")]
        public string Classification { get; set; }

        /// <summary>
        /// The REST method of the log entry
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; }

        /// <summary>
        /// The URI of the calling API
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The user agent of the log entry
        /// </summary>
        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// The IP address of calling API
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// The status code of the returned API
        /// </summary>
        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        /// <summary>
        /// The data associated with the API call
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; }

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
