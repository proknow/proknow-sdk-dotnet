using ProKnow.Scorecard;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Provides a summary of an entity scorecard
    /// </summary>
    public class EntityScorecardSummary : ScorecardTemplateSummary
    {
        private EntityScorecards _entityScorecards;

        /// <summary>
        /// Used by deserialization to create an entity scorecard summary
        /// </summary>
        public EntityScorecardSummary() : base()
        {
        }

        /// <summary>
        /// Gets the full representation of the entity scorecard asynchronously
        /// </summary>
        /// <returns>The full representation of an entity scorecard</returns>
        public override Task<ScorecardTemplateItem> GetAsync()
        {
            return ConvertToBaseTask(_entityScorecards.GetAsync(Id));
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
        /// <param name="entityScorecards">Interacts with scorecards for an entity in a ProKnow organization</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, EntityScorecards entityScorecards)
        {
            _proKnow = proKnow;
            _entityScorecards = entityScorecards;
        }

        /// <summary>
        /// Helper to cast EntityScorecardItem task to ScorecardTemplateItem task
        /// </summary>
        /// <param name="task">The EntityScorecardItem task</param>
        /// <returns>A ScorecardTemplateItem task</returns>
        private static async Task<ScorecardTemplateItem> ConvertToBaseTask(Task<EntityScorecardItem> task)
        {
            var entityScorecardItem = await task;
            return entityScorecardItem as ScorecardTemplateItem;
        }
    }
}
