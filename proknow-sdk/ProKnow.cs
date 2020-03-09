using System.IO;
using System.Text.Json;
using ProKnow.CustomMetric;
using ProKnow.Patient;
using ProKnow.Scorecard;
using ProKnow.Upload;

namespace ProKnow
{
    /// <summary>
    /// Root object for interfacing with the ProKnow API
    /// </summary>
    public class ProKnow
    {
        /// <summary>
        /// Issues requests to the ProKnow API
        /// </summary>
        internal Requestor Requestor { get; private set; }

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

        //todo--Add Collections property

        /// <summary>
        /// The number of seconds to use as a buffer when renewing a lock for a draft structure set. As an example, the default value of 30
        /// means that the renewer will attempt to renew the lock 30 seconds before it actually expires
        /// </summary>
        public int LockRenewalBuffer { get; set; }

        /// <summary>
        /// Constructs a ProKnow object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="credentialsFile">The path to the ProKnow credentials JSON file</param>
        /// <param name="lockRenewalBuffer">The number of seconds to use as a buffer when renewing a lock for a draft structure set</param>
        public ProKnow(string baseUrl, string credentialsFile, int lockRenewalBuffer = 30)
        {
            using (StreamReader sr = new StreamReader(credentialsFile))
            {
                var proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
                ConstructorHelper(baseUrl, proKnowCredentials.Id, proKnowCredentials.Secret, lockRenewalBuffer);
            }
        }

        /// <summary>
        /// Constructs a ProKnow object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="credentialsId">The ID from the ProKnow credentials JSON file</param>
        /// <param name="credentialsSecret">The secret from the ProKnow credentials JSON file</param>
        /// <param name="lockRenewalBuffer">The number of seconds to use as a buffer when renewing a lock for a draft structure set</param>
        public ProKnow(string baseUrl, string credentialsId, string credentialsSecret, int lockRenewalBuffer = 30)
        {
            ConstructorHelper(baseUrl, credentialsId, credentialsSecret, lockRenewalBuffer);
        }

        /// <summary>
        /// Helper to construct a ProKnow object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="credentialsId">The ID from the ProKnow credentials JSON file</param>
        /// <param name="credentialsSecret">The secret from the ProKnow credentials JSON file</param>
        /// <param name="lockRenewalBuffer">The number of seconds to use as a buffer when renewing a lock for a draft structure set</param>
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
            //todo--Initialize Collections property
        }
    }
}
