using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Role
{
    /// <summary>
    /// Represents the summary for a role for a ProKnow organization
    /// </summary>
    public class RoleSummary
    {
        private ProKnowApi _proKnow;

        /// <summary>
        /// The ProKnow ID of the role
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the role
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The description of the role
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Gets the full representation of the role asynchronously
        /// </summary>
        /// <returns>The full representaion of the role</returns>
        /// <example>This example shows how to get the full representation of a role:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var roleSummary = await _proKnow.Roles.FindAsync(x => x.Name == "Researcher");
        /// var roleItem = await roleSummary.GetAsync(roleSummary.Id);
        /// </code>
        /// </example>
        public Task<RoleItem> GetAsync()
        {
            return _proKnow.Roles.GetAsync(Id);
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
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
