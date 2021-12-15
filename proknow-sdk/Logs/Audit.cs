using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Logs
{
    /// <summary>
    /// Interacts with audit logging for a ProKnow organization.  This class is instantiated as an attribute of the
    /// ProKnow.ProKnowApi class
    /// </summary>
    public class Audit
    {
        private readonly ProKnowApi _proKnow;
        private FilterParametersExtended filterParameters = new FilterParametersExtended();
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };

        /// <summary>
        /// Constructs a Audit object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Audit(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Gets audit logs asynchronously
        /// </summary>
        /// <returns>A page of audit logs</returns>
        /// <example>This example shows how to get the first page of audit logs:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// FilterParameters filterParams = new FilterParameters();
        /// var auditLogs = await _proKnow.Audit.Query(filterParams);
        /// </code>
        /// </example>
        public async Task<AuditPage> Query(FilterParameters filter)
        {
            this.filterParameters.Copy(filter);

            this.filterParameters.PageNumber = null;
            if (filter == null)
            {
                this.filterParameters.PageSize = 25;
            }

            var bodyJson = JsonSerializer.Serialize(filterParameters, serializerOptions);
            var requestContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var json = await _proKnow.Requestor.PostAsync("audit/events/search", null, requestContent);
            var page = JsonSerializer.Deserialize<AuditPage>(json);

            if (page.Items.Count > 0)
            {
                this.filterParameters.FirstId = page.Items[0].Id;
                this.filterParameters.PageNumber = 0;
            }

            return page;
        }

        /// <summary>
        /// Gets next page of audit logs asynchronously
        /// </summary>
        /// <returns>The next page of audit logs</returns>
        /// <example>This example shows how to get the next page of audit logs:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var auditLogs = await _proKnow.Audit.Next();
        /// </code>
        /// </example>
        public async Task<AuditPage> Next()
        {
            if (this.filterParameters.FirstId == null)
            {
                throw new ProKnowException("Must call Query first");
            }

            ++this.filterParameters.PageNumber;

            var bodyJson = JsonSerializer.Serialize( this.filterParameters, serializerOptions);
            var requestContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var json = await _proKnow.Requestor.PostAsync("audit/events/search", null, requestContent);
            var auditItem = JsonSerializer.Deserialize<AuditPage>(json);

            return auditItem;
        }

        private class FilterParametersExtended
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
}
