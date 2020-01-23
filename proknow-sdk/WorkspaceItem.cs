using System.Collections.Generic;
using ProKnow.Tools;

namespace ProKnow
{
    /// <summary>
    /// Represents a workspace in a ProKnow organization
    /// </summary>
    public class WorkspaceItem
    {
        /// <summary>
        /// The parent Workspaces object
        /// </summary>
        internal Workspaces Workspaces { get; set; }

        /// <summary>
        /// The ID of the workspace (read-only)
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// The workspace slug.  A string with a maximum length of 40 that matches the regular expression ^[a-z0-9][a-z0-9]*(-[a-z0-9]+)*$
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The workspace name.  A string wth a maximum length of 80
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the workspace should be protected from accidental deletion
        /// </summary>
        public bool Protected { get; set; }

        /// <summary>
        /// All workspace attributes
        /// </summary>
        public Dictionary<string, object> Data { get; internal set; }

        /// <summary>
        /// Constructs a workspace item
        /// </summary>
        /// <param name="workspaces">The parent Workspaces object</param>
        /// <param name="data">The workspace item attributes</param>
        public WorkspaceItem(Workspaces workspaces, Dictionary<string, object> data)
        {
            Workspaces = workspaces;
            Id = JsonTools.DeserializeString(data, "id");
            Slug = JsonTools.DeserializeString(data, "slug");
            Name = JsonTools.DeserializeString(data, "name");
            Protected = JsonTools.DeserializeBoolean(data, "protected", true);
            Data = data;
        }
    }
}
