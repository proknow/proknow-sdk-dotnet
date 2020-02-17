using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        internal Workspaces Workspaces { get; set; }

        /// <summary>
        /// The workspace ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The workspace slug.  A string with a maximum length of 40 that matches the regular expression ^[a-z0-9][a-z0-9]*(-[a-z0-9]+)*$
        /// </summary>
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        /// <summary>
        /// The workspace name.  A string wth a maximum length of 80
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the workspace should be protected from accidental deletion
        /// </summary>
        [JsonPropertyName("protected")]
        public bool Protected { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="workspaces">The parent Workspaces object</param>
        internal void PostProcessDeserialization(Workspaces workspaces)
        {
            Workspaces = workspaces;
        }
    }
}
