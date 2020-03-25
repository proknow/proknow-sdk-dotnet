using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Collection
{
    /// <summary>
    /// Represents a collection
    /// </summary>
    public class CollectionItem
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
        /// The collection type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The collection workspace ID(s)
        /// </summary>
        [JsonPropertyName("workspaces")]
        public IList<string> WorkspaceIds { get; set; }

        /// <summary>
        /// Interacts with patients in this collection
        /// </summary>
        [JsonIgnore]
        public CollectionPatients Patients { get; private set; }

        //todo--add CollectionScorecards

        /// <summary>
        /// Used by deserialization to create a collection
        /// </summary>
        public CollectionItem()
        {
        }

        /// <summary>
        /// Creates a collection from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="json">JSON representation of the patient item</param>
        internal CollectionItem(ProKnow proKnow, string json)
        {
            var collectionItem = JsonSerializer.Deserialize<CollectionItem>(json);
            _proKnow = proKnow;
            Id = collectionItem.Id;
            Name = collectionItem.Name;
            Description = collectionItem.Description;
            Type = collectionItem.Type;
            WorkspaceIds = collectionItem.WorkspaceIds;
            Patients = new CollectionPatients(_proKnow, this);
        }

        /// <summary>
        /// Deletes this collection asynchronously
        /// </summary>
        public Task DeleteAsync()
        {
            return _proKnow.Collections.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves name and description changes to this collection asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            var properties = new Dictionary<string, object>();
            properties.Add("name", Name);
            properties.Add("description", Description);
            var requestJson = JsonSerializer.Serialize(properties);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/collections/{Id}", null, requestContent);
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
            Patients = new CollectionPatients(proKnow, this);
        }
    }
}
