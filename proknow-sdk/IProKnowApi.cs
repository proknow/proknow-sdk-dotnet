using System.Threading.Tasks;
using ProKnow.Collection;
using ProKnow.Audit;
using ProKnow.Patient;
using ProKnow.Role;
using ProKnow.Scorecard;
using ProKnow.Upload;
using ProKnow.User;

namespace ProKnow
{
    /// <summary>
    /// The interface for the main class that should be instantiated during application startup with your base URL (which should
    /// include your account subdomain) and your API token credentials
    /// </summary>
    public interface IProKnowApi
    {
        /// <summary>
        /// Interacts with audit logs in the ProKnow organization
        /// </summary>
        Audits Audit { get; }

        /// <summary>
        /// Interacts with collections in the ProKnow organization
        /// </summary>
        Collections Collections { get; }

        /// <summary>
        /// Interacts with custom metrics in the ProKnow organization
        /// </summary>
        CustomMetrics CustomMetrics { get; }

        /// <summary>
        /// The number of seconds to use as a buffer when renewing a lock for a draft structure set. As an example, the
        /// default value of 30 means that the renewer will attempt to renew the lock 30 seconds before it actually
        /// expires
        /// </summary>
        int LockRenewalBuffer { get; set; }

        /// <summary>
        /// Interacts with patients in the ProKnow organization
        /// </summary>
        Patients Patients { get; }

        /// <summary>
        /// Issues requests to the ProKnow API
        /// </summary>
        Requestor Requestor { get; }

        /// <summary>
        /// Issues requests to the RTV API
        /// </summary>
        RtvRequestor RtvRequestor { get; }

        /// <summary>
        /// Interacts with roles in the ProKnow organization
        /// </summary>
        Roles Roles { get; }

        /// <summary>
        /// Interacts with scorecard templates in the ProKnow organization
        /// </summary>
        ScorecardTemplates ScorecardTemplates { get; }

        /// <summary>
        /// Interacts with uploads in the ProKnow organization
        /// </summary>
        Uploads Uploads { get; }

        /// <summary>
        /// Interacts with users in the ProKnow organization
        /// </summary>
        Users Users { get; }

        /// <summary>
        /// Interacts with workspaces in the ProKnow organization
        /// </summary>
        Workspaces Workspaces { get; }

        /// <summary>
        /// Gets the ProKnow credentials (API key) status specified by the base URL and credentials (API key)
        /// asynchronously
        /// </summary>
        /// <returns>The ProKnow credentials (API key) status</returns>
        /// <example>This example shows how to verify that the provided base URL and credentials are valid: 
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        /// 
        /// var pk = new ProKnowApi('https://example.proknow.com', './credentials.json');
        /// var status = await pk.GetCredentialsStatusAsync();
        /// if (!status.IsValid)
        /// {
        ///     throw new Exception($"Error validating credentials (API key): {status.ErrorMessage}.");
        /// }
        /// </code>
        /// </example>
        Task<ProKnowCredentialsStatus> GetCredentialsStatusAsync();

        /// <summary>
        /// Gets the ProKnow domain status asynchronously
        /// </summary>
        /// <returns>The ProKnow domain status</returns>
        /// <remarks>
        /// If the ProKnow domain is up and reachable, IsOK will be true regardless of the provided base URL and credentials.
        /// </remarks>
        /// <example>This example shows how to verify that the ProKnow domain is up and reachable:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        /// 
        /// var pk = new ProKnowApi('https://example.proknow.com', './credentials.json');
        /// var status = await pk.GetDomainStatusAsync();
        /// if (!status.IsOk)
        /// {
        ///     throw new Exception($"Error connecting to ProKnow: {status.ErrorMessage}.");
        /// }
        /// </code>
        /// </example>
        Task<ProKnowDomainStatus> GetDomainStatusAsync();
    }
}
