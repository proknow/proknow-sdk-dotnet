using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace proknow_sdk
{
    /// <summary>
    /// Interacts with workspaces in a ProKnow organization
    /// </summary>
    public class Workspaces
    {
        private Requestor _requestor;

        /// <summary>
        /// Constructs a workspaces object
        /// </summary>
        /// <param name="requestor">Requestor for issuing requests to the ProKnow API</param>
        public Workspaces(Requestor requestor)
        {
            _requestor = requestor;
        }

        /// <summary>
        /// Queries the ProKnow API for the collection of workspace items
        /// </summary>
        /// <returns>A collection of workspace items</returns>
        public Task<IList<WorkspaceItem>> Query()
        {
            Task<string> workspacesJson = _requestor.getAsync("/workspaces");
            workspacesJson.Wait();
            return workspacesJson.ContinueWith(t => JsonSerializer.Deserialize<IList<WorkspaceItem>>(t.Result));
        }
    }
}
