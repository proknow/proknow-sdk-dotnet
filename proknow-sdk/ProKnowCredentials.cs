using System.Text.Json.Serialization;

namespace ProKnow
{
    /// <summary>
    /// POCO for contents of credentials JSON file from ProKnow
    /// </summary>
    internal class ProKnowCredentials
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }
    }
}
