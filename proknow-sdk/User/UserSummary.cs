using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.User
{
    /// <summary>
    /// Represents a summary of a user for a ProKnow organization
    /// </summary>
    public class UserSummary
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
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Gets the full representation of this user asynchronously
        /// </summary>
        /// <returns>The full representation of this user</returns>
        /// <example>This example shows how to get the full representation of a user named "Jane Doe":
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var userSummary = await _proKnow.Users.FindAsync(x => x.Name == "Jane Doe");
        /// var userItem = await userSummary.GetAsync(userSummary.Id);
        /// </code>
        /// </example>
        public Task<UserItem> GetAsync()
        {
            return _proKnow.Users.GetAsync(Id);
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
