using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Interacts with scorecards for an entity in a ProKnow organization
    /// </summary>
    public class EntityScorecards
    {
        private ProKnow _proKnow;
        private string _workspaceId;
        private string _entityId;

        /// <summary>
        /// Creates an entity scorecards object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="entityId">The entity ProKnow ID</param>
        public EntityScorecards(ProKnow proKnow, string workspaceId, string entityId)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _entityId = entityId;
        }

        /// <summary>
        /// Deletes an entity scorecard asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the entity scorecard</param>
        public async Task DeleteAsync(string id)
        {
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{_workspaceId}/entities/{_entityId}/metrics/sets/{id}");
        }

        /// <summary>
        /// Gets an entity scorecard item asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID for the entity scorecard</param>
        /// <returns>The entity scorecard item</returns>
        public async Task<EntityScorecardItem> GetAsync(string id)
        {
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{_workspaceId}/entities/{_entityId}/metrics/sets/{id}");
            return new EntityScorecardItem(_proKnow, _workspaceId, _entityId, this, json);
        }
    }
}
