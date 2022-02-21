using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.User
{
    /// <summary>
    /// Interacts with users for a ProKnow organization.  This class is instantiated as an attribute of the
    /// ProKnow.ProKnowApi class
    /// </summary>
    public class Users
    {
        private readonly ProKnowApi _proKnow;

        /// <summary>
        /// Constructs a users object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow object</param>
        internal Users(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a user asynchronously
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <param name="name">The name of the user</param>
        /// <param name="password">The optional password of the user</param>
        /// <returns>The created user</returns>
        /// <example>This example shows how to create a user:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userItem = await _proKnow.Users.CreateAsync("jane.doe@gmail.com", "Jane Doe");
        /// </code>
        /// </example>
        public async Task<UserItem> CreateAsync(string email, string name, string password = null)
        {
            var properties = new Dictionary<string, object>() { { "email", email }, { "name", name } };
            if (password != null)
            {
                properties.Add("password", password);
            }
            var requestContent = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync("/users", null, requestContent);
            var userItem = JsonSerializer.Deserialize<UserItem>(responseJson);
            userItem.PostProcessDeserialization(_proKnow);
            return userItem;
        }

        /// <summary>
        /// Deletes a user asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the user</param>
        /// <example>This example shows how to delete a user named "Jane Doe":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userSummary = await _proKnow.Users.FindAsync(x => x.Name == "Jane Doe");
        /// await _proKnow.Users.DeleteAsync(userSummary.Id);
        /// </code>
        /// </example>
        public Task DeleteAsync(string id)
        {
            return _proKnow.Requestor.DeleteAsync($"/users/{id}");
        }

        /// <summary>
        /// Finds the first user satisfying a predicate asynchronously
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first user that satisfies the predicate or null if the predicate was null or no user satisfies
        /// the predicate</returns>
        /// <remarks>
        /// For more information on how to use this method, see [Using Find Methods](../articles/usingFindMethods.md)
        /// </remarks>
        public async Task<UserSummary> FindAsync(Func<UserSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            var userSummaries = await QueryAsync();
            foreach (var userSummary in userSummaries)
            {
                if (predicate(userSummary))
                {
                    return userSummary;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a user asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the user</param>
        /// <returns>The specified user</returns>
        /// <example>This example shows how to get the full representation of a user named "Jane Doe":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userSummary = await _proKnow.Users.FindAsync(x => x.Name == "Jane Doe");
        /// var userItem = await _proKnow.Users.GetAsync(userSummary.Id);
        /// </code>
        /// </example>
        public async Task<UserItem> GetAsync(string id)
        {
            var json = await _proKnow.Requestor.GetAsync($"/users/{id}");
            var userItem = JsonSerializer.Deserialize<UserItem>(json);
            userItem.PostProcessDeserialization(_proKnow);
            return userItem;
        }

        /// <summary>
        /// Queries for users asynchronously
        /// </summary>
        /// <returns>The collection of users for the organization</returns>
        /// <example>This example shows how to get summaries for all of the users:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userSummaries = await _proKnow.Users.QueryAsync();
        /// </code>
        /// </example>
        public async Task<IList<UserSummary>> QueryAsync()
        {
            var json = await _proKnow.Requestor.GetAsync("/users");
            return DeserializeUserSummaries(json);
        }

        /// <summary>
        /// Creates a collection of user summaries from their JSON representation
        /// </summary>
        /// <param name="json">JSON representation of a collection of user summaries</param>
        /// <returns>A collection of user summaries</returns>
        private IList<UserSummary> DeserializeUserSummaries(string json)
        {
            var userSummaries = JsonSerializer.Deserialize<IList<UserSummary>>(json);
            foreach (var userSummary in userSummaries)
            {
                if (userSummary != null)
                {
                    userSummary.PostProcessDeserialization(_proKnow);
                }
            }
            return userSummaries;
        }
    }
}
