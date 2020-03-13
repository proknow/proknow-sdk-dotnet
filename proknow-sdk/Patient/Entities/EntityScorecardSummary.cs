using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ProKnow.Scorecard;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Provides a summary of an entity scorecard
    /// </summary>
    public class EntityScorecardSummary
    {
        private EntityScorecards _entityScorecards;

        /// <summary>
        /// The ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create an entity scorecard summary
        /// </summary>
        protected EntityScorecardSummary()
        {
        }

        /// <summary>
        /// Creates an entity scorecard summary from its JSON representation
        /// </summary>
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        /// <param name="json">JSON representation of the scorecard template summary</param>
        internal EntityScorecardSummary(EntityScorecards entityScorecards, string json)
        {
            var entityScorecardSummary = JsonSerializer.Deserialize<EntityScorecardSummary>(json);
            _entityScorecards = entityScorecards;
            Id = entityScorecardSummary.Id;
            Name = entityScorecardSummary.Name;
        }

        /// <summary>
        /// Gets the full representation of the scorecard template asynchronously
        /// </summary>
        /// <returns>The full representation of a scorecard template</returns>
        public Task<EntityScorecardItem> GetAsync()
        {
            return _entityScorecards.GetAsync(Id);
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
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        internal void PostProcessDeserialization(EntityScorecards entityScorecards)
        {
            _entityScorecards = entityScorecards;
        }
    }
}
