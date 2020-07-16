using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow
{
    /// <summary>
    /// Interacts with workspaces in a ProKnow organization
    /// </summary>
    public class Workspaces
    {
        private readonly ProKnowApi _proKnow;
        private IList<WorkspaceItem> _cache = null;

        /// <summary>
        /// Constructs a workspaces object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Workspaces(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a workspace asynchronously
        /// </summary>
        /// <param name="slug">The workspace slug. A string with a maximum length of 40 that matches the regular
        /// expression ``^[a-z0-9] [a-z0-9]* (-[a-z0-9]+)*$``</param>
        /// <param name="name">The workspace name. A string with a maximum length of 80</param>
        /// <param name="isProtected">Indicates whether the workspace should be protected from accidental
        /// deletion</param>
        /// <returns>The new workspace item</returns>
        public async Task<WorkspaceItem> CreateAsync(string slug, string name, bool isProtected = true)
        {
            var workspaceItem = new WorkspaceItem { Slug = slug, Name = name, Protected = isProtected };
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var content = new StringContent(JsonSerializer.Serialize(workspaceItem, jsonSerializerOptions),
                Encoding.UTF8, "application/json");
            string workspaceJson = await _proKnow.Requestor.PostAsync("/workspaces", null, content);
            _cache = null;
            workspaceItem = DeserializeWorkspace(workspaceJson);
            return workspaceItem;
        }

        /// <summary>
        /// Deletes a workspace asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        public async Task DeleteAsync(string workspaceId)
        {
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{workspaceId}");
            _cache = null;
        }

        /// <summary>
        /// Finds a workspace item asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first workspace item that satisfies the predicate or null if the predicate was null or no workspace 
        /// item satisfies the predicate</returns>
        public async Task<WorkspaceItem> FindAsync(Func<WorkspaceItem, bool> predicate)
        {
            if (_cache == null)
            {
                await QueryAsync();
            }
            return Find(predicate);
        }

        /// <summary>
        /// Queries asynchronously for the collection of workspace items
        /// </summary>
        /// <returns>A collection of workspace items</returns>
        public async Task<IList<WorkspaceItem>> QueryAsync()
        {
            string workspacesJson = await _proKnow.Requestor.GetAsync("/workspaces");
            _cache = DeserializeWorkspaces(workspacesJson);
            return _cache.ToList();
        }

        /// <summary>
        /// Resolves a workspace asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace</param>
        /// <returns>The workspace item corresponding to the specified ID or name</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
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
        /// Resolves a workspace by its ProKnow ID asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the workspace</param>
        /// <returns>The workspace item corresponding to the specified ID</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        public async Task<WorkspaceItem> ResolveByIdAsync(string workspaceId)
        {
            if (String.IsNullOrWhiteSpace(workspaceId))
            {
                throw new ArgumentException("The workspace ProKnow ID must be specified.");
            }
            var workspaceItem = await FindAsync(t => t.Id == workspaceId);
            if (workspaceItem == null)
            {
                throw new ProKnowWorkspaceLookupException($"There is no workspace with a ProKnow ID of '{workspaceId}'.");
            }
            return workspaceItem;
        }

        /// <summary>
        /// Resolves a workspace by its name asynchronously
        /// </summary>
        /// <param name="workspaceName">The name of the workspace</param>
        /// <returns>The workspace item corresponding to the specified name</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        public async Task<WorkspaceItem> ResolveByNameAsync(string workspaceName)
        {
            if (String.IsNullOrWhiteSpace(workspaceName))
            {
                throw new ArgumentException("The workspace name must be specified.");
            }
            var workspaceItem = await FindAsync(t => t.Name == workspaceName);
            if (workspaceItem == null)
            {
                throw new ProKnowWorkspaceLookupException($"There is no workspace with a Name of '{workspaceName}'.");
            }
            return workspaceItem;
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
        /// Creates a collection of workspace items from their JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the workspace items</param>
        /// <returns>A collection of workspace items</returns>
        private IList<WorkspaceItem> DeserializeWorkspaces(string json)
        {
            var workspaceItems = JsonSerializer.Deserialize<IList<WorkspaceItem>>(json);
            foreach (var workspaceItem in workspaceItems)
            {
                workspaceItem.PostProcessDeserialization(_proKnow);
            }
            return workspaceItems;
        }

        /// <summary>
        /// Creates a workspace item from its JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the workspace item</param>
        /// <returns>The workspace item</returns>
        private WorkspaceItem DeserializeWorkspace(string json)
        {
            var workspaceItem = JsonSerializer.Deserialize<WorkspaceItem>(json);
            workspaceItem.PostProcessDeserialization(_proKnow);
            return workspaceItem;
        }
    }
}
