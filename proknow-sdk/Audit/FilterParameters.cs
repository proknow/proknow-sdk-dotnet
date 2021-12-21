using System;
using System.Text.Json.Serialization;

namespace ProKnow.Audit
{
    /// <summary>
    /// Represents a class of possible parameters that can be used to filter
    /// audit log results
    /// </summary>
    public class FilterParameters
    {
        /// <summary>
        /// The number of items for each page
        /// </summary>
        [JsonPropertyName("page_size")]
        public uint? PageSize { get; set; }

        /// <summary>
        /// Start date cut off for whole query
        /// </summary>
        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// End date cut off for whole query
        /// </summary>
        [JsonPropertyName("end_time")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// The type of event
        /// </summary>
        [JsonPropertyName("types")]
        public string[] Types { get; set; }

        /// <summary>
        /// Name of event's enacting user
        /// </summary>
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// Name of patient
        /// </summary>
        [JsonPropertyName("patient_name")]
        public string PatientName { get; set; }

        /// <summary>
        /// 'HTTP" or 'AUTH'
        /// </summary>
        [JsonPropertyName("classification")]
        public string Classification { get; set; }

        /// <summary>
        /// HTTP Method: 'GET', 'HEAD', 'POST', 'PUT', 'DELETE', 'CONNECT', 'OPTIONS', 'TRACE', or 'PATCH'
        /// </summary>
        [JsonPropertyName("methods")]
        public string[] Methods { get; set; }

        /// <summary>
        /// Filter by the URI that event took place (e.g. '/metrics/custom')
        /// </summary>
        [JsonPropertyName("uri")]
        public string URI { get; set; }

        /// <summary>
        /// User agent attributed to event (e.g. Browser User Agent)
        /// </summary>
        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// IP address attributed to event
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Status code returned by the api call
        /// </summary>
        [JsonPropertyName("status_codes")]
        public string[] StatusCodes { get; set; }

        /// <summary>
        /// ID of workspace in which event took place
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public string WorkspaceId { get; set; }

        /// <summary>
        /// ID of resource
        /// </summary>
        [JsonPropertyName("resource_id")]
        public string ResourceId { get; set; }
    }
}
