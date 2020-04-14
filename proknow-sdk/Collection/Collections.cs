using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProKnow.Collection
{
    /// <summary>
    /// Interacts with collections in a ProKnow organization
    /// </summary>
    public class Collections
    {
        private ProKnowApi _proKnow;

        /// <summary>
        /// Constructs a collections object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal Collections(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a collection asynchronously
        /// </summary>
        /// <param name="name">The collection name</param>
        /// <param name="description">The collection description</param>
        /// <param name="type">The collection type (either "workspace" or "organization")</param>
        /// <param name="workspaceIds">The ProKnow IDs for the workspaces in the collection.  For workspace collections,
        /// there must be exactly one workspace</param>
        /// <returns>The created collection</returns>
        public async Task<CollectionItem> CreateAsync(string name, string description, string type, IList<string> workspaceIds)
        {
            var properties = new Dictionary<string, object>();
            properties.Add("name", name);
            properties.Add("description", description);
            properties.Add("type", type);
            properties.Add("workspaces", workspaceIds);
            var requestJson = JsonSerializer.Serialize(properties);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync($"/collections", null, requestContent);
            return new CollectionItem(_proKnow, responseJson);
        }

        /// <summary>
        /// Deletes a collection asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the collection</param>
        public async Task DeleteAsync(string id)
        {
            await _proKnow.Requestor.DeleteAsync($"/collections/{id}");
        }

        /// <summary>
        /// Finds a collection asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace or null to consider only organization
        /// collections</param>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first collection that satisfies the predicate or null if the predicate was null or no
        /// collection satisfies the predicate</returns>
        public async Task<CollectionSummary> FindAsync(string workspace, Func<CollectionSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            var patientSummaries = await QueryAsync(workspace);
            foreach (var patientSummary in patientSummaries)
            {
                if (predicate(patientSummary))
                {
                    return patientSummary;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a collection asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the collection</param>
        /// <returns>The collection</returns>
        public async Task<CollectionItem> GetAsync(string id)
        {
            var json = await _proKnow.Requestor.GetAsync($"/collections/{id}");
            return new CollectionItem(_proKnow, json);
        }

        /// <summary>
        /// Queries for collections asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace or null to query for only organization
        /// collections</param>
        /// <returns>Summaries of the collections found</returns>
        public async Task<IList<CollectionSummary>> QueryAsync(string workspace = null)
        {
            var queryParameters = new Dictionary<string, object>();
            if (workspace != null)
            {
                var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
                if (workspaceItem != null)
                {
                    queryParameters.Add("workspace", workspaceItem.Id);
                }
            }
            var json = await _proKnow.Requestor.GetAsync($"/collections", queryParameters);
            return DeserializeCollections(json);
        }

        /// <summary>
        /// Creates a collection of collection summaries from their JSON representation
        /// </summary>
        /// <param name="json">JSON representation of a collection of collection summaries</param>
        /// <returns>A collection of collection summaries</returns>
        private IList<CollectionSummary> DeserializeCollections(string json)
        {
            var collectionSummaries = JsonSerializer.Deserialize<IList<CollectionSummary>>(json);
            foreach (var collectionSummary in collectionSummaries)
            {
                if (collectionSummary != null)
                {
                    collectionSummary.PostProcessDeserialization(_proKnow);
                }
            }
            return collectionSummaries;
        }
    }
}
