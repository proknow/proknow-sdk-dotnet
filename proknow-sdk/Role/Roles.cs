using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Role
{
    /// <summary>
    /// Interacts with roles for a ProKnow organization.  This class is instantiated as an attribute of the
    /// ProKnow.ProKnowApi class
    /// </summary>
    public class Roles
    {
        private readonly ProKnowApi _proKnow;

        /// <summary>
        /// Constructs a roles object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Roles(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a role asynchronously
        /// </summary>
        /// <param name="name">The name of the role</param>
        /// <param name="permissions">The permissions of the role</param>
        /// <param name="description">The description of the role</param>
        /// <returns>The created role</returns>
        /// <example>This example shows how to create a role named "Researcher" with permissions to read patients:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var permissions = new Permissions(canReadPatients: true);
        /// var roleItem = await _proKnow.Roles.CreateAsync("Researcher", "Description", permissions);
        /// </code>
        /// </example>
        public async Task<RoleItem> CreateAsync(string name, string description, Permissions permissions)
        {
            var roleItemToCreate = new Dictionary<string, object>() { { "name", name }, { "description", description }, { "permissions", permissions } };
            var requestContent = new StringContent(JsonSerializer.Serialize(roleItemToCreate), Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync("/roles", null, requestContent);
            var createdRoleItem =  JsonSerializer.Deserialize<RoleItem>(responseJson);
            createdRoleItem.PostProcessDeserialization(_proKnow);
            return createdRoleItem;
        }

        /// <summary>
        /// Deletes a role asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the role</param>
        /// <example>This example shows how to delete a role named "Researcher":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var roleSummary = await _proKnow.Roles.FindAsync(x => x.Name == "Researcher");
        /// await _proKnow.Roles.DeleteAsync(roleSummary.Id);
        /// </code>
        /// </example>
        public Task DeleteAsync(string id)
        {
            return _proKnow.Requestor.DeleteAsync($"/roles/{id}");
        }

        /// <summary>
        /// Finds the first role satisfying a predicate asynchronously
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first role that satisfies the predicate or null if the predicate was null or no role satisfies
        /// the predicate</returns>
        /// <remarks>
        /// For more information on how to use this method, see [Using Find Methods](../articles/usingFindMethods.md)
        /// </remarks>
        public async Task<RoleSummary> FindAsync(Func<RoleSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            var roleSummaries = await QueryAsync();
            foreach (var roleSummary in roleSummaries)
            {
                if (predicate(roleSummary))
                {
                    return roleSummary;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a role asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the role</param>
        /// <returns>The specified role</returns>
        /// <example>This example shows how to get the full representation of a role:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var roleSummary = await _proKnow.Roles.FindAsync(x => x.Name == "Researcher");
        /// var roleItem = await _proKnow.Roles.GetAsync(roleSummary.Id);
        /// </code>
        /// </example>
        public async Task<RoleItem> GetAsync(string id)
        {
            var json = await _proKnow.Requestor.GetAsync($"/roles/{id}");
            var roleItem = JsonSerializer.Deserialize<RoleItem>(json);
            roleItem.PostProcessDeserialization(_proKnow);
            return roleItem;
        }

        /// <summary>
        /// Queries for roles asynchronously
        /// </summary>
        /// <returns>The collection of roles for the organization</returns>
        /// <example>This example shows how to get summaries for all of the roles:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var roleSummaries = await _proKnow.Roles.QueryAsync();
        /// </code>
        /// </example>
        public async Task<IList<RoleSummary>> QueryAsync()
        {
            var json = await _proKnow.Requestor.GetAsync("/roles");
            return DeserializeRoleSummaries(json);
        }

        /// <summary>
        /// Creates a collection of role summaries from their JSON representation
        /// </summary>
        /// <param name="json">JSON representation of a collection of role summaries</param>
        /// <returns>A collection of role summaries</returns>
        private IList<RoleSummary> DeserializeRoleSummaries(string json)
        {
            var roleSummaries = JsonSerializer.Deserialize<IList<RoleSummary>>(json);
            foreach (var roleSummary in roleSummaries)
            {
                if (roleSummary != null)
                {
                    roleSummary.PostProcessDeserialization(_proKnow);
                }
            }
            return roleSummaries;
        }
    }
}
