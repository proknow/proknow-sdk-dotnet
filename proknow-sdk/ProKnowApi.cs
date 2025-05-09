using Microsoft.Extensions.Logging;
using ProKnow.Collection;
using ProKnow.Exceptions;
using ProKnow.Patient;
using ProKnow.Role;
using ProKnow.Scorecard;
using ProKnow.Upload;
using ProKnow.User;
using ProKnow.Audit;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Reflection;

[assembly: InternalsVisibleTo("proknow-sdk-test")]

namespace ProKnow
{
    /// <summary>
    /// This is the main class that should be instantiated during application startup with your base URL (which should
    /// include your account subdomain) and your API token credentials
    /// </summary>
    public class ProKnowApi : IProKnowApi
    {
        private readonly ILogger _logger;

        private static Version _version = Assembly.GetExecutingAssembly().GetName().Version;

        internal RtvRequestor RtvRequestor { get; private set; }

        /// <inheritdoc/>
        public Audits Audit { get; private set; }

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
            catch (DirectoryNotFoundException ex)
            {
                var message = $"The credentials file '{credentialsFile}' was not found.";
                _logger.LogError(ex, message);
                throw new ProKnowException(message, ex);
            }
            catch (FileNotFoundException ex)
            {
                var message = $"The credentials file '{credentialsFile}' was not found.";
                _logger.LogError(ex, message);
                throw new ProKnowException(message, ex);
            }
            catch (Exception ex)
            {
                var message = $"The credentials file '{credentialsFile}' is not valid JSON.";
                _logger.LogError(ex, message);
                throw new ProKnowException(message, ex);
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
        public async Task<ProKnowCredentialsStatus> GetCredentialsStatusAsync()
        {
            try
            {
                await Requestor.GetAsync("/user");
                return new ProKnowCredentialsStatus(true);
            }
            catch (ProKnowHttpException ex)
            {
                return new ProKnowCredentialsStatus(false, ex.Message, ex.ResponseStatusCode, ex);
            }
            catch (Exception ex)
            {
                return new ProKnowCredentialsStatus(false, ex.Message, null, ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ProKnowDomainStatus> GetDomainStatusAsync()
        {
            try
            {
                await Requestor.GetDomainStatusAsync();
                return new ProKnowDomainStatus(true);
            }
            catch (Exception ex)
            {
                return new ProKnowDomainStatus(false, ex.Message, ex);
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
            var userAgent = new KeyValuePair<string, string>(
                "User-Agent",
                $"ProKnow-SDK-dotnet/v{_version.Major}.{_version.Minor}.{_version.Build}"
            );
            Requestor = new Requestor(
                baseUrl, 
                credentialsId, 
                credentialsSecret, 
                new List<KeyValuePair<string, string>> { userAgent }
            );
            RtvRequestor = new RtvRequestor(baseUrl, new List<KeyValuePair<string, string>>
            {
                userAgent
            });
            CustomMetrics = new CustomMetrics(this);
            ScorecardTemplates = new ScorecardTemplates(this);
            Workspaces = new Workspaces(this);
            Roles = new Roles(this);
            Users = new Users(this);
            Uploads = new Uploads(this);
            Patients = new Patients(this);
            Collections = new Collections(this);
            Audit = new Audits(this);
        }
    }
}
