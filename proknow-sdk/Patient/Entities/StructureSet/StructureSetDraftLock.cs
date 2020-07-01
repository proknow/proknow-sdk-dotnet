using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents a structure set draft lock
    /// </summary>
    public class StructureSetDraftLock
    {
        /// <summary>
        /// The ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The timestamp at which the lock was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// The timestamp at which the lock expires
        /// </summary>
        [JsonPropertyName("expires_at")]
        public string ExpiresAt { get; set; }

        /// <summary>
        /// The number of milliseconds in which the lock expires
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
