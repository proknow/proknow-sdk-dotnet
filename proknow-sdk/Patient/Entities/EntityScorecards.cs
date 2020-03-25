using ProKnow.Scorecard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        /// Constructs an EntityScorecards object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="entityId">The ProKnow ID for the entity</param>
        public EntityScorecards(ProKnow proKnow, string workspaceId, string entityId)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _entityId = entityId;
        }

        /// <summary>
        /// Creates an entity scorecard asynchronously
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics (names and objectives)</param>
        /// <returns>The created entity scorecard</returns>
        public async Task<EntityScorecardItem> CreateAsync(string name, IList<ComputedMetric> computedMetrics,
            IList<CustomMetricItem> customMetrics)
        {
            // Check arguments
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (computedMetrics == null)
            {
                throw new ArgumentNullException("computedMetrics");
            }
            if (customMetrics == null)
            {
                throw new ArgumentNullException("customMetrics");
            }

            // Resolve custom metrics (obtain their IDs) and add objectives
            var resolvedCustomMetrics = new List<CustomMetricItem>();
            var tasks = new List<Task>();
            foreach (var inputCustomMetric in customMetrics)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var resolvedCustomMetric = await _proKnow.CustomMetrics.ResolveByNameAsync(inputCustomMetric.Name);
                    resolvedCustomMetric.Objectives = inputCustomMetric.Objectives;
                    resolvedCustomMetrics.Add(resolvedCustomMetric);
                }));
            }
            await Task.WhenAll(tasks);

            // Convert custom metrics to their scorecard template creation schema
            var customMetricIdsAndObjectives = resolvedCustomMetrics.Select(c => c.ConvertToScorecardSchema()).ToList();

            // Request the creation
            var route = $"/workspaces/{_workspaceId}/entities/{_entityId}/metrics/sets";
            var requestSchema = new EntityScorecardItem(null, null, null, null, null, name, computedMetrics, customMetricIdsAndObjectives);
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var contentJson = JsonSerializer.Serialize(requestSchema, jsonSerializerOptions);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            string responseJson = await _proKnow.Requestor.PostAsync(route, null, content);

            // Return the created entity scorecard, with complete custom metric representations
            var responseSchema = JsonSerializer.Deserialize<ScorecardTemplateItem>(responseJson);
            return new EntityScorecardItem(_proKnow, _workspaceId, _entityId, this, responseSchema.Id, responseSchema.Name,
                responseSchema.ComputedMetrics, resolvedCustomMetrics);
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
        /// Finds an entity scorecard asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first entity scorecard that satisfies the predicate or null if the predicate was null or no
        /// entity scorecard satisfies the predicate</returns>
        public async Task<EntityScorecardSummary> FindAsync(Func<EntityScorecardSummary, bool> predicate)
        {
            var entityScorecards = await QueryAsync();
            return Find(entityScorecards, predicate);
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

        /// <summary>
        /// Queries for entity scorecards asynchronously
        /// </summary>
        /// <returns>The entity scorecards</returns>
        public async Task<IList<EntityScorecardSummary>> QueryAsync()
        {
            string json = await _proKnow.Requestor.GetAsync($"/workspaces/{_workspaceId}/entities/{_entityId}/metrics/sets");
            return DeserializeEntityScorecards(json);
        }

        /// <summary>
        /// Finds an entity scorecard based on a predicate
        /// </summary>
        /// <param name="entityScorecards">The entity scorecards to search</param>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first entity scorecard that satisfies the predicate or null if the predicate was null or no
        /// entity scorecard that satisfies the predicate was found</returns>
        private EntityScorecardSummary Find(IList<EntityScorecardSummary> entityScorecards,
            Func<EntityScorecardSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            foreach (var entityScorecard in entityScorecards)
            {
                if (predicate(entityScorecard))
                {
                    return entityScorecard;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a collection of entity scorecards from their JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the entity scorecards</param>
        /// <returns>A collection of entity scorecards</returns>
        private IList<EntityScorecardSummary> DeserializeEntityScorecards(string json)
        {
            var entityScorecards = JsonSerializer.Deserialize<IList<EntityScorecardSummary>>(json);
            foreach (var entityScorecard in entityScorecards)
            {
                entityScorecard.PostProcessDeserialization(_proKnow, this);
            }
            return entityScorecards;
        }
    }
}
