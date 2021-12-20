using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        private FilterParametersExtended _filterParameters = new FilterParametersExtended();
        private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
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
            this._filterParameters.Copy(filter);

            this._filterParameters.PageNumber = null;
            if (filter == null)
            {
                this._filterParameters.PageSize = 25;
            }

            var bodyJson = JsonSerializer.Serialize(_filterParameters, _serializerOptions);
            var requestContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var json = await _proKnow.Requestor.PostAsync("audit/events/search", null, requestContent);
            var page = JsonSerializer.Deserialize<AuditPage>(json);

            if (page.Items.Count > 0)
            {
                this._filterParameters.FirstId = page.Items[0].Id;
                this._filterParameters.PageNumber = 0;
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
        /// await _proKnow.Audit.Query(filterParams);
        /// var auditLogs = await _proKnow.Audit.Next();
        /// </code>
        /// </example>
        public async Task<AuditPage> Next()
        {
            if (this._filterParameters.FirstId == null)
            {
                throw new ProKnowException("Must call Query first");
            }

            ++this._filterParameters.PageNumber;

            var bodyJson = JsonSerializer.Serialize( this._filterParameters, _serializerOptions);
            var requestContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var json = await _proKnow.Requestor.PostAsync("audit/events/search", null, requestContent);
            var auditItem = JsonSerializer.Deserialize<AuditPage>(json);

            return auditItem;
        }       
    }
}
