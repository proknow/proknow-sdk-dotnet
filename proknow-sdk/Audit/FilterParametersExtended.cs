using System;
using System.Text.Json.Serialization;

namespace ProKnow.Logs
{
    internal class FilterParametersExtended
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
        /// Start date cut off for whole query
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
        /// Filter by the uri that event took place (e.g. '/metrics/custom')
        /// </summary>
        [JsonPropertyName("uri")]
        public string URI { get; set; }

        /// <summary>
        /// User Agent attributed to event (e.g. Browser User Agent)
        /// </summary>
        [JsonPropertyName("user_agent")]
        public string UserAgent { get; set; }

        /// <summary>
        /// IP Address attributed to event
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Status codes returned by API calls
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

        /// <summary>
        /// Page number of request
        /// </summary>
        [JsonPropertyName("page_number")]
        public uint? PageNumber { get; set; }

        /// <summary>
        /// First ID of request
        /// </summary>
        [JsonPropertyName("first_id")]
        public string FirstId { get; set; }

        public void Copy(FilterParameters data)
        {
            this.PageSize = data.PageSize;
            this.StartTime = data.StartTime;
            this.EndTime = data.EndTime;
            this.Types = data.Types;
            this.UserName = data.UserName;
            this.PatientName = data.PatientName;
            this.Classification = data.Classification;
            this.Methods = data.Methods;
            this.URI = data.URI;
            this.UserAgent = data.UserAgent;
            this.IpAddress = data.IpAddress;
            this.StatusCodes = data.StatusCodes;
            this.WorkspaceId = data.WorkspaceId;
            this.ResourceId = data.ResourceId;
        }
    }
}
