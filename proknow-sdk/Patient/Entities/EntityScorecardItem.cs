using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ProKnow.CustomMetric;
using ProKnow.Scorecard;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents an entity scorecard
    /// </summary>
    public class EntityScorecardItem
    {
        private ProKnow _proKnow;
        private string _workspaceId;
        private string _entityId;
        private EntityScorecards _entityScorecards;

        /// <summary>
        /// The scorecard ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The computed metrics
        /// </summary>
        [JsonPropertyName("computed")]
        public IList<ComputedMetric> ComputedMetrics { get; set; }

        /// <summary>
        /// The custom metrics
        /// </summary>
        [JsonPropertyName("custom")]
        public IList<CustomMetricItem> CustomMetrics { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create entity scorecard item
        /// </summary>
        protected EntityScorecardItem()
        {
        }

        /// <summary>
        /// Creates a scorecard template
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        /// <param name="id">The entity scorecard ProKnow ID</param>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics</param>
        internal EntityScorecardItem(ProKnow proKnow, string workspaceId, string entityId,
            EntityScorecards entityScorecards, string id, string name, IList<ComputedMetric> computedMetrics,
            IList<CustomMetricItem> customMetrics)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _entityId = entityId;
            _entityScorecards = entityScorecards;
            Id = id;
            Name = name;
            ComputedMetrics = computedMetrics;
            CustomMetrics = customMetrics;
        }

        /// <summary>
        /// Creates a entity scorecard item from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        /// <param name="json">JSON representation of the entity scorecard item</param>
        internal EntityScorecardItem(ProKnow proKnow, string workspaceId, string entityId,
            EntityScorecards entityScorecards, string json)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _entityId = entityId;
            _entityScorecards = entityScorecards;
            var entityScorecardItem = JsonSerializer.Deserialize<EntityScorecardItem>(json);
            Id = entityScorecardItem.Id;
            Name = entityScorecardItem.Name;
            ComputedMetrics = entityScorecardItem.ComputedMetrics;
            CustomMetrics = entityScorecardItem.CustomMetrics;
        }

        /// <summary>
        /// Deletes this entity scorecard item instance asynchronously
        /// </summary>
        public async Task DeleteAsync()
        {
            await _entityScorecards.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves changes to an entity scorecard asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            var route = $"/workspaces/{_workspaceId}/entities/{_entityId}/metrics/sets/{Id}";
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var contentJson = JsonSerializer.Serialize(ConvertToSaveSchema(), jsonSerializerOptions);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync(route, null, content);
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
        /// Provide a copy of this instance containing only the information required to represent it in a save request
        /// </summary>
        /// <returns>A copy of this instance containing only the information required to represent it in a save
        /// request</returns>
        internal EntityScorecardItem ConvertToSaveSchema()
        {
            return new EntityScorecardItem()
            {
                Name = Name,
                ComputedMetrics = ComputedMetrics,
                CustomMetrics = CustomMetrics.Select(c => c.ConvertToScorecardTemplateSchema()).ToList()
            };
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        internal void PostProcessDeserialization(ProKnow proKnow, string workspaceId, string entityId,
            EntityScorecards entityScorecards)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _entityId = entityId;
            _entityScorecards = entityScorecards;
        }
    }
}
