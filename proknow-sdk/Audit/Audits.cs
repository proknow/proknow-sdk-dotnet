using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Audit
{
    /// <summary>
    /// Interacts with audit logging for a ProKnow organization.  This class is instantiated as an attribute of the
    /// ProKnow.ProKnowApi class
    /// </summary>
    public class Audits
    {
        private readonly ProKnowApi _proKnow;
        private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
        };

        /// <summary>
        /// Constructs a Audit object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Audits(ProKnowApi proKnow)
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
        /// var page = await _proKnow.Audit.Query(filterParams);
        /// var nextPage = await page.Next();
        /// </code>
        /// </example>
        public async Task<AuditPage> Query(FilterParameters filter)
        {
            FilterParametersExtended filterParameters = new FilterParametersExtended();

            filterParameters.Copy(filter);

            filterParameters.PageNumber = null;
            if (filter == null)
            {
                filterParameters.PageSize = 25;
            }

            var bodyJson = JsonSerializer.Serialize(filterParameters, _serializerOptions);
            var requestContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            var json = await _proKnow.Requestor.PostAsync("/audit/events/search", null, requestContent);
            var page = JsonSerializer.Deserialize<AuditPage>(json);

            if (page.Items.Count > 0)
            {
                filterParameters.FirstId = page.Items[0].Id;
                filterParameters.PageNumber = 0;
            }

            page.PostProcessDeserialization(_proKnow, filterParameters);
            return page;
        }
    }
}
