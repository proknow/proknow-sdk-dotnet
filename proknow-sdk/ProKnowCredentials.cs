using System.Text.Json.Serialization;

namespace ProKnow
{
    /// <summary>
    /// Represents the contents of a credentials JSON file from ProKnow
    /// </summary>
    internal class ProKnowCredentials
    {
        /// <summary>
        /// The ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The secret
        /// </summary>
        [JsonPropertyName("secret")]
        public string Secret { get; set; }
    }
}
