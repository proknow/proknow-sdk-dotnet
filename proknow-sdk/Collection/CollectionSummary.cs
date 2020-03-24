using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Collection
{
    /// <summary>
    /// Provides a summary of a collection
    /// </summary>
    public class CollectionSummary
    {
        private ProKnow _proKnow;

        /// <summary>
        /// The ProKnow ID of the collection
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The collection name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The collection description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets the complete representation of the collection
        /// </summary>
        /// <returns>The complete representation of the collection</returns>
        public Task<CollectionItem> GetAsync()
        {
            return _proKnow.Collections.GetAsync(Id);
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }
    }
}
