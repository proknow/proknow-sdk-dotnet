using System.Text.Json.Serialization;

namespace ProKnow
{
    /// <summary>
    /// Represents a workspace in a ProKnow organization
    /// </summary>
    public class WorkspaceItem
    {
        /// <summary>
        /// The ID of the workspace (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; internal set; }

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
    }
}
