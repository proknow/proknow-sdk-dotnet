using ProKnow.Collection;
using ProKnow.Exceptions;
using ProKnow.Patient;
using ProKnow.Scorecard;
using ProKnow.Upload;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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
        /// <exception cref="ProKnow.Exceptions.ProKnowException">If the credentials file could not be read or is invalid</exception>
        public ProKnowApi(string baseUrl, string credentialsFile, int lockRenewalBuffer = 30)
        {
            ProKnowCredentials proKnowCredentials = null;
            try
            {
                using (StreamReader sr = new StreamReader(credentialsFile))
                {
                    proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
                }
            }
            catch (FileNotFoundException)
            {
                throw new ProKnowException($"The credentials file '{credentialsFile}' was not found.");
            }
            catch (Exception)
            {
                throw new ProKnowException($"The credentials file '{credentialsFile}' is not valid JSON.");
            }
            if (proKnowCredentials.Id == null || proKnowCredentials.Secret == null)
            {
                throw new ProKnowException($"The 'id' and/or 'secret' in the credentials file '{credentialsFile}' are missing.");
            }
            ConstructorHelper(baseUrl, proKnowCredentials.Id, proKnowCredentials.Secret, lockRenewalBuffer);
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
        /// Gets the status of the API connection asynchronously
        /// </summary>
        /// <returns>"OK" for a valid API connection; otherwise an error message indicating the issue</returns>
        /// <example>
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        /// 
        /// var pk = new ProKnowApi('https://example.proknow.com', './credentials.json');
        /// var status = await pk.GetStatusAsync();
        /// if (status != "OK")
        /// {
        ///     throw new Exception($"Error connecting to ProKnow API: {status}.");
        /// }
        /// </code>
        /// </example>
        public async Task<string> GetStatusAsync()
        {
            string status = null;
            try
            {
                status = await Requestor.GetAsync("/status");
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            return status;
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
