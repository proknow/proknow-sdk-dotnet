using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow
{
    /// <summary>
    /// Represents a workspace in a ProKnow organization
    /// </summary>
    public class WorkspaceItem
    {
        private ProKnowApi _proKnow;

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
        /// Deletes this workspace asynchronously
        /// </summary>
        public Task DeleteAsync()
        {
            return _proKnow.Workspaces.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves slug, name, and protected flag changes asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            var properties = new Dictionary<string, object>
            {
                { "slug", Slug },
                { "name", Name },
                { "protected", Protected }
            };
            var content = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/workspaces/{Id}", null, content);
        }

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
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }
    }
}
