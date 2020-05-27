using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Collection
{
    /// <summary>
    /// Interacts with patients for a collection
    /// </summary>
    public class CollectionPatients
    {
        private ProKnowApi _proKnow;
        private CollectionItem _collectionItem;

        /// <summary>
        /// Constructs a CollectionPatients object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="collectionItem">The collection</param>
        public CollectionPatients(ProKnowApi proKnow, CollectionItem collectionItem)
        {
            _proKnow = proKnow;
            _collectionItem = collectionItem;
        }

        /// <summary>
        /// Adds items to a collection asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace in which to the find the patients</param>
        /// <param name="items">The ProKnow IDs of the patients and optional entities to add</param>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        public async Task AddAsync(string workspace, IList<CollectionPatientsAddSchema> items)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var route = $"/collections/{_collectionItem.Id}/workspaces/{workspaceItem.Id}/patients";
            var requestJson = JsonSerializer.Serialize(items);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync(route, null, requestContent);
        }

        /// <summary>
        /// Queries for patients belonging to the collection asynchronously
        /// </summary>
        /// <returns>The patients belonging to the collection</returns>
        public async Task<IList<CollectionPatientSummary>> QueryAsync()
        {
            var route = $"/collections/{_collectionItem.Id}/patients";
            Dictionary<string, object> queryParameters = null;
            if (_collectionItem.Type == "workspace")
            {
                queryParameters = new Dictionary<string, object>();
                queryParameters.Add("workspace", _collectionItem.WorkspaceIds[0]);
            }
            var responseJson = await _proKnow.Requestor.GetAsync(route, queryParameters);
            return DeserializeCollectionPatientSummaries(responseJson);
        }

        /// <summary>
        /// Removes patients from a collection asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace in which to the find the patients</param>
        /// <param name="patientIds">The ProKnow IDs of the patients to remove</param>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        public async Task RemoveAsync(string workspace, IList<string> patientIds)
        {
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);
            var route = $"/collections/{_collectionItem.Id}/workspaces/{workspaceItem.Id}/patients";
            var items = new List<Dictionary<string, object>>();
            foreach (var patientId in patientIds)
            {
                items.Add(new Dictionary<string, object>() { { "patient", patientId } });
            }
            var requestJson = JsonSerializer.Serialize(items);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.DeleteAsync(route, null, requestContent);
        }

        /// <summary>
        /// Creates a collection of collection patient summaries from their JSON representation
        /// </summary>
        /// <param name="json">JSON representation of a collection of collection patient summaries</param>
        /// <returns>A collection of collection patient summaries</returns>
        private IList<CollectionPatientSummary> DeserializeCollectionPatientSummaries(string json)
        {
            var collectionPatientSummaries = JsonSerializer.Deserialize<IList<CollectionPatientSummary>>(json);
            foreach (var collectionPatientSummary in collectionPatientSummaries)
            {
                if (collectionPatientSummary != null)
                {
                    collectionPatientSummary.PostProcessDeserialization(_proKnow);
                }
            }
            return collectionPatientSummaries;
        }
    }
}
