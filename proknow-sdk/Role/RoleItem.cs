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
        public async Task SaveAsync()
        {
            var content = new StringContent(RoleItem.Serialize(Name, Permissions), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/roles/{Id}", null, content);
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
        /// Deserializes a role from a JSON string with the permissions at the root level as provided by the API
        /// </summary>
        /// <param name="json">The JSON string to deserialize</param>
        /// <returns>A role</returns>
        internal static RoleItem Deserialize(string json)
        {
            // Deserialize
            var role = JsonSerializer.Deserialize<RoleItem>(json);

            // Extract permissions from extension data
            ExtractPermissionsFromExtensionData(role);

            return role;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Serializes a role to a JSON string with the permissions at the root level as required by the API
        /// </summary>
        /// <param name="name">The role name</param>
        /// <param name="permissions">The role permissions</param>
        /// <returns>A JSON string representation of this object</returns>
        internal static string Serialize(string name, OrganizationPermissions permissions)
        {
            // Convert permissions to Dictionary<string, object>
            var properties = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(permissions));

            // Add role name to dictionary
            properties["name"] = name;

            return JsonSerializer.Serialize(properties);
        }

        /// <summary>
        /// Extracts the permissions from the ExtensionData
        /// </summary>
        /// <param name="role">The role to fix</param>
        private static void ExtractPermissionsFromExtensionData(RoleItem role)
        {
            // Serialize the ExtensionData back to a dictionary and then deserialize it into permissions
            role.Permissions = JsonSerializer.Deserialize<OrganizationPermissions>(JsonSerializer.Serialize(role.ExtensionData));

            // Remove the permissions from the ExtensionData so they won't overwrite the property values when this object is
            // serialized
            role.ExtensionData.Remove("create_api_keys");
            role.ExtensionData.Remove("manage_access");
            role.ExtensionData.Remove("manage_custom_metrics");
            role.ExtensionData.Remove("manage_template_metric_sets");
            role.ExtensionData.Remove("manage_renaming_rules");
            role.ExtensionData.Remove("manage_template_checklists");
            role.ExtensionData.Remove("organization_collaborator");
            role.ExtensionData.Remove("organization_read_patients");
            role.ExtensionData.Remove("organization_read_collections");
            role.ExtensionData.Remove("organization_view_phi");
            role.ExtensionData.Remove("organization_download_dicom");
            role.ExtensionData.Remove("organization_write_collections");
            role.ExtensionData.Remove("organization_write_patients");
            role.ExtensionData.Remove("organization_contour_patients");
            role.ExtensionData.Remove("organization_delete_collections");
            role.ExtensionData.Remove("organization_delete_patients");
            role.ExtensionData.Remove("workspaces");
        }
    }
}
