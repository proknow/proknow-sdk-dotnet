using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProKnow
{
    /// <summary>
    /// Interacts with workspaces in a ProKnow organization
    /// </summary>
    public class Workspaces
    {
        private ProKnow _proKnow;
        private IList<WorkspaceItem> _cache = null;

        /// <summary>
        /// Constructs a workspaces object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        public Workspaces(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }
        
        /// <summary>
        /// Finds a workspace item asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first workspace item that satisfies the predicate or null if the predicate was null or no workspace 
        /// item satisfies the predicate</returns>
        public Task<WorkspaceItem> FindAsync(Func<WorkspaceItem, bool> predicate)
        {
            if (_cache == null)
            {
                return QueryAsync().ContinueWith(_ => Find(predicate));
            }
            return Task.FromResult(Find(predicate));
        }

        /// <summary>
        /// Queries asynchronously for the collection of workspace items
        /// </summary>
        /// <returns>A collection of workspace items</returns>
        public Task<IList<WorkspaceItem>> QueryAsync()
        {
            Task<string> workspacesJson = _proKnow.Requestor.GetAsync("/workspaces");
            return workspacesJson.ContinueWith(t => HandleQueryResponse(t.Result));
        }

        /// <summary>
        /// Resolves a workspace asynchronously
        /// </summary>
        /// <param name="workspace">The ID or name of the workspace</param>
        /// <returns>The workspace item corresponding to the specified ID or name or null if no matching workspace was found</returns>
        public Task<WorkspaceItem> ResolveAsync(string workspace)
        {
            Regex regex = new Regex(@"^[0-9a-f]{32}$");
            Match match = regex.Match(workspace);
            if (match.Success)
            {
                return ResolveByIdAsync(workspace);
            } 
            else
            {
                return ResolveByNameAsync(workspace);
            }
        }

        /// <summary>
        /// Resolves a workspace ID asynchronously
        /// </summary>
        /// <param name="workspaceId">The ID of the workspace</param>
        /// <returns>The workspace item corresponding to the specified ID or null if no matching workspace was found</returns>
        public Task<WorkspaceItem> ResolveByIdAsync(string workspaceId)
        {
            if (String.IsNullOrWhiteSpace(workspaceId))
            {
                throw new ArgumentException("The workspace ID must be specified.");
            }
            return FindAsync(t => t.Id == workspaceId);
        }

        /// <summary>
        /// Resolves a workspace name asynchronously
        /// </summary>
        /// <param name="workspaceName">The name of the workspace</param>
        /// <returns>The workspace item corresponding to the specified name or null if no matching workspace was found</returns>
        public Task<WorkspaceItem> ResolveByNameAsync(string workspaceName)
        {
            if (String.IsNullOrWhiteSpace(workspaceName))
            {
                throw new ArgumentException("The workspace name must be specified.");
            }
            return FindAsync(t => t.Name == workspaceName);
        }

        /// <summary>
        /// Finds a workspace item based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
         /// <returns>The first workspace item that satisfies the predicate or null if the predicate was null or no workspace
         /// item satisfies the predicate were found</returns>
        private WorkspaceItem Find(Func<WorkspaceItem, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            foreach (var workspaceItem in _cache)
            {
                if (predicate(workspaceItem))
                {
                    return workspaceItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Handles the query response
        /// </summary>
        /// <param name="workspaceJson">The JSON representation of the workspace items</param>
        /// <returns>A collection of workspace items</returns>
        private IList<WorkspaceItem> HandleQueryResponse(string workspaceJson)
        {
            _cache = DeserializeWorkspaces(workspaceJson);
            return _cache;
        }

        /// <summary>
        /// Creates a collection of workspace items from their JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the workspace items</param>
        /// <returns>A collection of workspace items</returns>
        private IList<WorkspaceItem> DeserializeWorkspaces(string json)
        {
            var workspaceItems = JsonSerializer.Deserialize<IList<WorkspaceItem>>(json);
            foreach (var workspaceItem in workspaceItems)
            {
                workspaceItem.PostProcessDeserialization(this);
            }
            return workspaceItems;
        }
    }
}
