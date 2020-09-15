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
    [JsonConverter(typeof(RoleItemJsonConverter))]
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
        /// Read-only flag indicating whether role is private
        /// </summary>
        [JsonPropertyName("private")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The permissions of the role
        /// </summary>
        [JsonIgnore]
        public OrganizationPermissions Permissions { get; set; }

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
            Permissions = new OrganizationPermissions();
        }

        /// <summary>
        /// Constructs a RoleItem.  Used internally to create a public role
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="permissions">The permissions</param>
        internal RoleItem(string name, OrganizationPermissions permissions)
        {
            Name = name;
            Permissions = permissions;
        }

        /// <summary>
        /// Constructs a RoleItem.  Used to create a private role for a user
        /// </summary>
        /// <param name="permissions">The permissions</param>
        public RoleItem(OrganizationPermissions permissions)
        {
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
        /// roleItem.Permissions.CanManageCustomMetrics = true;
        /// await roleItem.SaveAsync();
        /// </code>
        /// </example>
        public Task SaveAsync()
        {
            var content = new StringContent(JsonSerializer.Serialize(this), Encoding.UTF8, "application/json");
            return _proKnow.Requestor.PutAsync($"/roles/{Id}", null, content);
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
