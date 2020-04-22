using ProKnow.Scorecard;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents an entity scorecard
    /// </summary>
    public class EntityScorecardItem : ScorecardTemplateItem
    {
        private string _workspaceId;
        private string _entityId;
        private EntityScorecards _entityScorecards;

        /// <summary>
        /// Used by deserialization to create entity scorecard item
        /// </summary>
        public EntityScorecardItem() : base()
        {
        }

        /// <summary>
        /// Creates an entity scorecard item
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        /// <param name="id">The entity scorecard ProKnow ID</param>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics</param>
        internal EntityScorecardItem(ProKnowApi proKnow, string workspaceId, string entityId,
            EntityScorecards entityScorecards, string id, string name, IList<ComputedMetric> computedMetrics,
            IList<CustomMetricItem> customMetrics) : base(proKnow, id, name, computedMetrics, customMetrics)
        {
            _workspaceId = workspaceId;
            _entityId = entityId;
            _entityScorecards = entityScorecards;
        }

        /// <summary>
        /// Creates a entity scorecard item from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        /// <param name="json">JSON representation of the entity scorecard item</param>
        internal EntityScorecardItem(ProKnowApi proKnow, string workspaceId, string entityId,
            EntityScorecards entityScorecards, string json) : base(proKnow, json)
        {
            _workspaceId = workspaceId;
            _entityId = entityId;
            _entityScorecards = entityScorecards;
        }

        /// <summary>
        /// Deletes this entity scorecard item instance asynchronously
        /// </summary>
        public override async Task DeleteAsync()
        {
            await _entityScorecards.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves changes to an entity scorecard asynchronously
        /// </summary>
        public override async Task SaveAsync()
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
        internal override ScorecardTemplateItem ConvertToSaveSchema()
        {
            return new EntityScorecardItem()
            {
                Name = Name,
                ComputedMetrics = ComputedMetrics,
                CustomMetrics = CustomMetrics.Select(c => c.ConvertToScorecardSchema()).ToList()
            };
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId, string entityId,
            EntityScorecards entityScorecards)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _entityId = entityId;
            _entityScorecards = entityScorecards;
        }
    }
}
