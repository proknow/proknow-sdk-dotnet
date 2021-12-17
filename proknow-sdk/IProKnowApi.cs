using System.Threading.Tasks;
using ProKnow.Collection;
using ProKnow.Logs;
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
        /// Interacts with roles in the ProKnow organization
        /// </summary>
        Audit Audit { get; }

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
        /// Gets the status of the API connection asynchronously
        /// </summary>
        /// <returns>The connection status</returns>
        /// <example>
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        /// 
        /// var pk = new ProKnowApi('https://example.proknow.com', './credentials.json');
        /// var status = await pk.GetConnectionStatusAsync();
        /// if (!connectionStatus.IsValid)
        /// {
        ///     throw new Exception($"Error connecting to ProKnow API: {connectionStatus.ErrorMessage}.");
        /// }
        /// </code>
        /// </example>
        Task<ProKnowConnectionStatus> GetConnectionStatusAsync();
    }
}
