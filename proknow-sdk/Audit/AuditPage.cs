using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using ProKnow.Exceptions;

namespace ProKnow.Audit
{
    /// <summary>
    /// Represents a collection of audit log items
    /// </summary>
    public class AuditPage
    {
        private ProKnowApi _proKnow;
        private FilterParametersExtended _filterParameters;

        private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };

        /// <summary>
        /// The total number of log entries
        /// </summary>
        [JsonPropertyName("total")]
        public uint Total { get; set; }

        /// <summary>
        /// The audit log items returned
        /// </summary>
        [JsonPropertyName("items")]
        public List<AuditItem> Items { get; set; }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="parameters">The filter paramters used in API call</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, FilterParametersExtended parameters)
        {
            _proKnow = proKnow;
            _filterParameters = new FilterParametersExtended(parameters);

            //Increment page number for next page call.
            ++this._filterParameters.PageNumber;
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
        /// var page = await _proKnow.Audit.Query(filterParams);
        /// var nextPage = await page.Next();
        /// </code>
        /// </example>
        public async Task<AuditPage> Next()
        {
            var bodyJson = JsonSerializer.Serialize(this._filterParameters, _serializerOptions);
            var requestContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var json = await _proKnow.Requestor.PostAsync("/audit/events/search", null, requestContent);
            var auditPage = JsonSerializer.Deserialize<AuditPage>(json);

            auditPage.PostProcessDeserialization(this._proKnow, this._filterParameters);

            return auditPage;
        }
    }
}
