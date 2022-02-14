using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Role
{
    /// <summary>
    /// Represents a role for a ProKnow organization
    /// </summary>
    public class RoleItem
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
        /// The permissions of the role
        /// </summary>
        [JsonPropertyName("permissions")]
        public Permissions Permissions { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create a role
        /// </summary>
        public RoleItem()
        {
            Permissions = new Permissions();
        }

        /// <summary>
        /// Constructs a RoleItem
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="description">The description</param>
        /// <param name="permissions">The permissions</param>
        internal RoleItem(string name, string description, Permissions permissions)
        {
            Name = name;
            Description = description;
            Permissions = permissions;
        }

        /// <summary>
        /// Deletes this role asynchronously
        /// </summary>
        /// <example>This example shows how to delete a role named "Researcher":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var roleSummary = await _proKnow.Roles.FindAsync(x => x.Name == "Researcher");
        /// var roleItem = await roleSummary.GetAsync(roleSummary.Id);
        /// await roleItem.DeleteAsync();
        /// </code>
        /// </example>
        public Task DeleteAsync()
        {
            return _proKnow.Roles.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves changes to a role asynchronously
        /// </summary>
        /// <example>This example shows how to save changes to a role named "Researcher":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var roleSummary = await _proKnow.Roles.FindAsync(x => x.Name == "Researcher");
        /// var roleItem = await roleSummary.GetAsync(roleSummary.Id);
        /// roleItem.Permissions.CanReadPatients = true;
        /// await roleItem.SaveAsync();
        /// </code>
        /// </example>
        public Task SaveAsync()
        {

            // Convert permissions to Dictionary<string, object> and add it to the object
            var permissions = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(this.Permissions));
            var roleItemToUpdate = new Dictionary<string, object>() {
                { "name", this.Name }, { "description", this.Description }, { "permissions", permissions }
            };
            var content = new StringContent(JsonSerializer.Serialize(roleItemToUpdate), Encoding.UTF8, "application/json");
            return _proKnow.Requestor.PatchAsync($"/roles/{Id}", null, content);
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
