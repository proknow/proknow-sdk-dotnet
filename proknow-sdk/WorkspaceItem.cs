using System;
using System.Collections.Generic;
using System.Text.Json;
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
        /// All workspace attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Determines whether this object satisfies a predicate and/or specified property values
        /// </summary>
        /// <param name="predicate">The optional predicate</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>True if this object satisfies the predicate (if specified) and all property filters (if specified); otherwise false</returns>
        public bool DoesMatch(Func<WorkspaceItem, bool> predicate = null, params KeyValuePair<string, object>[] properties)
        {
            if (predicate != null && !predicate(this))
            {
                return false;
            }
            foreach (var kvp in properties)
            {
                switch (kvp.Key)
                {
                    case "id":
                        if (!Id.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "slug":
                        if (!Slug.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "name":
                        if (!Name.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "protected":
                        if (!Protected.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    default:
                        if (!Data.ContainsKey(kvp.Key) || !Data[kvp.Key].Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
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
