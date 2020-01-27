﻿using System;
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
        /// Finds a workspace item asynchronously based on a predicate and/or properties
        /// </summary>
        /// <param name="predicate">An optional predicate for the search</param>
        /// <param name="properties">Optional property filters (values)</param>
        /// <returns>All workspace items that satisfy the predicate (if specified) and all property filters (if specified) or null
        /// if none were found or neither a predicate nor property filters were specified</returns>
        public Task<WorkspaceItem> FindAsync(Func<WorkspaceItem, bool> predicate = null, params KeyValuePair<string, object>[] properties)
        {
            if (_cache == null)
            {
                return QueryAsync().ContinueWith(_ => Find(predicate, properties));
            }
            return Task.FromResult(Find(predicate, properties));
        }

        /// <summary>
        /// Finds a workspace item based on a predicate and/or properties
        /// </summary>
        /// <param name="predicate">An optional predicate for the search</param>
        /// <param name="properties">Optional property filters (values)</param>
        /// <returns>All workspace items that satisfy the predicate (if specified) and all property filters (if specified) or null
        /// if none were found or neither a predicate nor property filters were specified</returns>
        private WorkspaceItem Find(Func<WorkspaceItem, bool> predicate = null, params KeyValuePair<string, object>[] properties)
        {
            if (predicate == null && properties == null)
            {
                return null;
            }
            foreach (var workspaceItem in _cache)
            {
                bool match = true;
                if (predicate != null && !predicate(workspaceItem))
                {
                    match = false;
                }
                else
                {
                    foreach (var kvp in properties)
                    {
                        if (!workspaceItem.Data.ContainsKey(kvp.Key) || !workspaceItem.Data[kvp.Key].Equals(kvp.Value))
                        {
                            match = false;
                            break;
                        }
                    }
                }
                if (match)
                {
                    return workspaceItem;
                }
            }
            return null;
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
