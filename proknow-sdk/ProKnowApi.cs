using ProKnow.Collection;
using ProKnow.Patient;
using ProKnow.Scorecard;
using ProKnow.Upload;
using System.IO;
using System.Text.Json;

namespace ProKnow
{
    /// <summary>
    /// This is the main class that should be instantiated during application startup with your base URL (which should
    /// include your account subdomain) and your API token credentials.
    /// </summary>
    public class ProKnowApi
    {
        /// <summary>
        /// Issues requests to the ProKnow API
        /// </summary>
        public Requestor Requestor { get; private set; }

        /// <summary>
        /// Interacts with custom metrics in the ProKnow organization
        /// </summary>
        public CustomMetrics CustomMetrics { get; private set; }

        /// <summary>
        /// Interacts with scorecard templates in the ProKnow organization
        /// </summary>
        public ScorecardTemplates ScorecardTemplates { get; private set; }

        /// <summary>
        /// Interacts with workspaces in the ProKnow organization
        /// </summary>
        public Workspaces Workspaces { get; private set; }

        //todo--Add Roles property

        //todo--Add Users property

        /// <summary>
        /// Interacts with uploads in the ProKnow organization
        /// </summary>
        public Uploads Uploads { get; private set; }

        /// <summary>
        /// Interacts with patients in the ProKnow organization
        /// </summary>
        public Patients Patients { get; private set; }

        /// <summary>
        /// Interacts with collections in the ProKnow organization
        /// </summary>
        public Collections Collections { get; private set; }

        /// <summary>
        /// The number of seconds to use as a buffer when renewing a lock for a draft structure set. As an example, the
        /// default value of 30 means that the renewer will attempt to renew the lock 30 seconds before it actually
        /// expires
        /// </summary>
        public int LockRenewalBuffer { get; set; }

        /// <summary>
        /// Constructs a ProKnowApi object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="credentialsFile">The path to the ProKnow credentials JSON file</param>
        /// <param name="lockRenewalBuffer">The number of seconds to use as a buffer when renewing a lock for a draft
        /// structure set</param>
        public ProKnowApi(string baseUrl, string credentialsFile, int lockRenewalBuffer = 30)
        {
            using (StreamReader sr = new StreamReader(credentialsFile))
            {
                var proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
                ConstructorHelper(baseUrl, proKnowCredentials.Id, proKnowCredentials.Secret, lockRenewalBuffer);
            }
        }

        /// <summary>
        /// Constructs a ProKnowApi object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="credentialsId">The ID from the ProKnow credentials JSON file</param>
        /// <param name="credentialsSecret">The secret from the ProKnow credentials JSON file</param>
        /// <param name="lockRenewalBuffer">The number of seconds to use as a buffer when renewing a lock for a draft
        /// structure set</param>
        public ProKnowApi(string baseUrl, string credentialsId, string credentialsSecret, int lockRenewalBuffer = 30)
        {
            ConstructorHelper(baseUrl, credentialsId, credentialsSecret, lockRenewalBuffer);
        }

        /// <summary>
        /// Helper to construct a ProKnowApi object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="credentialsId">The ID from the ProKnow credentials JSON file</param>
        /// <param name="credentialsSecret">The secret from the ProKnow credentials JSON file</param>
        /// <param name="lockRenewalBuffer">The number of seconds to use as a buffer when renewing a lock for a draft
        /// structure set</param>
        private void ConstructorHelper(string baseUrl, string credentialsId, string credentialsSecret, int lockRenewalBuffer)
        {
            LockRenewalBuffer = lockRenewalBuffer;
            Requestor = new Requestor(baseUrl, credentialsId, credentialsSecret);
            CustomMetrics = new CustomMetrics(this);
            ScorecardTemplates = new ScorecardTemplates(this);
            Workspaces = new Workspaces(this);
            //todo--Initialize Roles property
            //todo--Initialize Users property
            Uploads = new Uploads(this);
            Patients = new Patients(this);
            Collections = new Collections(this);
        }
    }
}
