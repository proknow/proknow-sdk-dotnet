using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Logs
{
    /// <summary>
    /// Represents a collection of audit log items
    /// </summary>
    public class AuditPage
    {
        private ProKnowApi _proKnow;

        /// <summary>
        /// The total number of log entries
        /// </summary>
        [JsonPropertyName("total")]
        public uint Total { get; set; }

        /// <summary>
        /// The audit log items returned
        /// </summary>
        [JsonPropertyName("items")]
        public List<AuditItem> Items { get; set; }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Get next page of audit logs
        /// </summary>
        public Task Next()
        {
            return this._proKnow.Audit.Next();
        }
    }
}
