using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Interacts with the versions of a structure set
    /// </summary>
    /// <remarks>
    /// Objects of this class are instantiated by the <see cref="StructureSetItem"/> class.
    /// </remarks>
    public class StructureSetVersions
    {
        private ProKnowApi _proKnow;

        /// <summary>
        /// The ProKnow ID of the workspace
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; protected set; }

        /// <summary>
        /// The ProKnow ID of the structure set item that instantiated this object
        /// </summary>
        public string StructureSetId { get; protected set; }

        /// <summary>
        /// Constructs a StructureSetVersions object
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The ProKnow ID of the workspace</param>
        /// <param name="structureSetId">The ProKnow ID of the structure set item that instantiated this object</param>
        public StructureSetVersions(ProKnowApi proKnow, string workspaceId, string structureSetId)
        {
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
            StructureSetId = structureSetId;
        }

        /// <summary>
        /// Deletes a structure set version asynchronously
        /// </summary>
        /// <param name="versionId"></param>
        /// <returns></returns>
        public Task DeleteAsync(string versionId)
        {
            return _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}/versions/{versionId}");
        }

        /// <summary>
        /// Gets a structure set version asynchronously
        /// </summary>
        /// <param name="versionId">The ProKnow ID of the structure set version</param>
        /// <returns>A structure set item for the given version</returns>
        public async Task<StructureSetItem> GetAsync(string versionId)
        {
            var queryParameters = new Dictionary<string, object>();
            queryParameters.Add("version", versionId);
            var responseJson = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}", queryParameters);
            var structureSetItem = JsonSerializer.Deserialize<StructureSetItem>(responseJson);
            structureSetItem.PostProcessDeserialization(_proKnow, WorkspaceId);
            structureSetItem.IsDraft = (versionId == "draft");
            return structureSetItem;
        }

        /// <summary>
        /// Queries for structure set versions asynchronously
        /// </summary>
        /// <returns>The collection of versions for the structure set item that instantiated this object</returns>
        public async Task<IList<StructureSetVersionItem>> QueryAsync()
        {
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}/versions");
            return JsonSerializer.Deserialize<IList<StructureSetVersionItem>>(json);
        }
    }
}
