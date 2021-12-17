using Microsoft.Extensions.Logging;
using ProKnow.Collection;
using ProKnow.Exceptions;
using ProKnow.Logs;
using ProKnow.Patient;
using ProKnow.Role;
using ProKnow.Scorecard;
using ProKnow.Upload;
using ProKnow.User;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow
{
    /// <summary>
    /// This is the main class that should be instantiated during application startup with your base URL (which should
    /// include your account subdomain) and your API token credentials
    /// </summary>
    public class ProKnowApi : IProKnowApi
    {
        private readonly ILogger _logger;

        /// <inheritdoc/>
        public Audit Audit { get; private set; }

        /// <inheritdoc/>
        public Requestor Requestor { get; private set; }

        /// <inheritdoc/>
        public CustomMetrics CustomMetrics { get; private set; }

        /// <inheritdoc/>
        public ScorecardTemplates ScorecardTemplates { get; private set; }

        /// <inheritdoc/>
        public Workspaces Workspaces { get; private set; }

        /// <inheritdoc/>
        public Roles Roles { get; private set; }

        /// <inheritdoc/>
        public Users Users { get; private set; }

        /// <inheritdoc/>
        public Uploads Uploads { get; private set; }

        /// <inheritdoc/>
        public Patients Patients { get; private set; }

        /// <inheritdoc/>
        public Collections Collections { get; private set; }

        /// <inheritdoc/>
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
            _logger = ProKnowLogging.CreateLogger(typeof(ProKnowApi).FullName);
            ProKnowCredentials proKnowCredentials = null;
            try
            {
                using (StreamReader sr = new StreamReader(credentialsFile))
                {
                    proKnowCredentials = JsonSerializer.Deserialize<ProKnowCredentials>(sr.ReadToEnd());
                }
            }
            catch (DirectoryNotFoundException)
            {
                var message = $"The credentials file '{credentialsFile}' was not found.";
                _logger.LogError(message);
                throw new ProKnowException(message);
            }
            catch (FileNotFoundException)
            {
                var message = $"The credentials file '{credentialsFile}' was not found.";
                _logger.LogError(message);
                throw new ProKnowException(message);
            }
            catch (Exception)
            {
                var message = $"The credentials file '{credentialsFile}' is not valid JSON.";
                _logger.LogError(message);
                throw new ProKnowException(message);
            }
            if (proKnowCredentials.Id == null || proKnowCredentials.Secret == null)
            {
                var message = $"The 'id' and/or 'secret' in the credentials file '{credentialsFile}' are missing.";
                _logger.LogError(message);
                throw new ProKnowException(message);
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
            _logger = ProKnowLogging.CreateLogger(typeof(ProKnowApi).FullName);
            ConstructorHelper(baseUrl, credentialsId, credentialsSecret, lockRenewalBuffer);
        }

        /// <inheritdoc/>
        public async Task<ProKnowConnectionStatus> GetConnectionStatusAsync()
        {
            try
            {
                await Requestor.GetAsync("/status");
                return new ProKnowConnectionStatus(true);
            }
            catch (Exception ex)
            {
                return new ProKnowConnectionStatus(false, ex.Message);
            }
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
            Roles = new Roles(this);
            Users = new Users(this);
            Uploads = new Uploads(this);
            Patients = new Patients(this);
            Collections = new Collections(this);
            Audit = new Audit(this);
        }
    }
}
