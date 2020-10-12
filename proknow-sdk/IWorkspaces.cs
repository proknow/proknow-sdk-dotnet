using ProKnow.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProKnow
{
    /// <summary>
    /// Interacts with workspaces in a ProKnow organization
    /// </summary>
    public interface IWorkspaces
    {
        /// <summary>
        /// Creates a workspace asynchronously
        /// </summary>
        /// <param name="slug">The workspace slug. A string with a maximum length of 40 that matches the regular
        /// expression ``^[a-z0-9] [a-z0-9]* (-[a-z0-9]+)*$``</param>
        /// <param name="name">The workspace name. A string with a maximum length of 80</param>
        /// <param name="isProtected">Indicates whether the workspace should be protected from accidental
        /// deletion</param>
        /// <returns>The new workspace item</returns>
        Task<WorkspaceItem> CreateAsync(string slug, string name, bool isProtected = true);

        /// <summary>
        /// Deletes a workspace asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        Task DeleteAsync(string workspaceId);

        /// <summary>
        /// Finds a workspace item asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first workspace item that satisfies the predicate or null if the predicate was null or no workspace 
        /// item satisfies the predicate</returns>
        Task<WorkspaceItem> FindAsync(Func<WorkspaceItem, bool> predicate);

        /// <summary>
        /// Queries asynchronously for the collection of workspace items
        /// </summary>
        /// <returns>A collection of workspace items</returns>
        Task<IList<WorkspaceItem>> QueryAsync();

        /// <summary>
        /// Resolves a workspace asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace</param>
        /// <returns>The workspace item corresponding to the specified ID or name</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        Task<WorkspaceItem> ResolveAsync(string workspace);

        /// <summary>
        /// Resolves a workspace by its ProKnow ID asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the workspace</param>
        /// <returns>The workspace item corresponding to the specified ID</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        Task<WorkspaceItem> ResolveByIdAsync(string workspaceId);

        /// <summary>
        /// Resolves a workspace by its name asynchronously
        /// </summary>
        /// <param name="workspaceName">The name of the workspace</param>
        /// <returns>The workspace item corresponding to the specified name</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        Task<WorkspaceItem> ResolveByNameAsync(string workspaceName);
    }
}
