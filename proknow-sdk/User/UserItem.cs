using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.User
{
    /// <summary>
    /// Represents a user for a ProKnow organization
    /// </summary>
    public class UserItem
    {
        private ProKnowApi _proKnow;

        /// <summary>
        /// The ProKnow ID of the user
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the user
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The email address of the user
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The flag indicating whether the user is active
        /// </summary>
        [JsonPropertyName("active")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Deletes this user asynchronously
        /// </summary>
        /// <example>This example shows how to delete a user named "Jane Doe":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userSummary = await _proKnow.Users.FindAsync(x => x.Name == "Jane Doe");
        /// var userItem = await userSummary.GetAsync();
        /// await userItem.DeleteAsync();
        /// </code>
        /// </example>
        public Task DeleteAsync()
        {
            return _proKnow.Users.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves changes to a user asynchronously
        /// </summary>
        /// <example>This example shows how to find a user by their email, set them to inactive, and save the change:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userSummary = await _proKnow.Users.FindAsync(x => x.Email == "jsmith@example.com");
        /// var userItem = await userSummary.GetAsync(userSummary.Id);
        /// userItem.IsActive = false;
        /// await userItem.SaveAsync();
        /// </code>
        /// </example>
        public Task SaveAsync()
        {
            var properties = new Dictionary<string, object>() { { "email", Email }, { "name", Name }, { "active", IsActive } };
            var content = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            return _proKnow.Requestor.PutAsync($"/users/{Id}", null, content);
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
