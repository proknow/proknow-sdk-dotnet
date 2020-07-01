using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents a region of interest (ROI) in a structure set
    /// </summary>
    public class StructureSetRoiItem
    {
        private ProKnowApi _proKnow;
        private StructureSetItem _structureSetItem;
        private JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// The ProKnow ID for the workspace
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The method used to generate the contour
        /// </summary>
        [JsonPropertyName("algorithm")]
        public string Algorithm { get; set; }

        /// <summary>
        /// The RGB components of the color
        /// </summary>
        [JsonPropertyName("color")]
        [JsonConverter(typeof(ColorJsonConverter))]
        public Color Color { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The number
        /// </summary>
        [JsonPropertyName("number")]
        public int Number { get; set; }

        /// <summary>
        /// The ProKnow tag
        /// </summary>
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// The type
        /// </summary>
        /// <remarks>
        /// The valid types are'EXTERNAL', 'PTV', 'CTV', 'GTV', 'TREATED_VOLUME', 'IRRAD_VOLUME', 'BOLUS', 'AVOIDANCE',
        /// 'ORGAN', 'MARKER', 'REGISTRATION', 'ISOCENTER', 'CONTRAST_AGENT', 'CAVITY', 'BRACHY_CHANNEL',
        /// 'BRACHY_ACCESSORY', 'BRACHY_SRC_APP', 'BRACHY_CHNL_SHLD', 'SUPPORT', 'FIXATION', 'DOSE_REGION', 'CONTROL'
        /// </remarks>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Deletes this ROI asynchronously
        /// </summary>
        /// <example>This example shows how to delete an ROI, commit the change, and refresh the structure set:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var workspaceItem = await _proKnow.Workspaces.FindAsync(w => w.Name == "Clinical");
        /// var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Name == "Example");
        /// var patientItem = await patientSummary.GetAsync();
        /// var structureSetItem = patientItem.FindEntities(e => e.Type == "structure_set")[0];
        /// using (var draft = await structureSetItem.DraftAsync())
        /// {
        ///     var roiItem = draft.Rois.First(r => r.Name == "PTV");
        ///     await roiItem.DeleteAsync();
        ///     await draft.ApproveAsync();
        /// }
        /// await structureSetItem.RefreshAsync();
        /// </code>
        /// </example>
        public async Task DeleteAsync()
        {
            if (!IsEditable())
            {
                throw new InvalidOperationError("Item is not editable");
            }
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Lock", _structureSetItem.DraftLock.Id) };
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/structuresets/{_structureSetItem.Id}/draft/rois/{Id}", headerKeyValuePairs);
            _structureSetItem.Rois = _structureSetItem.Rois.Where(r => r.Id != Id).ToArray();
        }

        //todo--Implement GetDataAsync method

        /// <summary>
        /// Indicates whether this ROI is editable
        /// </summary>
        /// <returns>True if this ROI is editable; otherwise false</returns>
        public bool IsEditable()
        {
            return _structureSetItem.IsEditable;
        }

        /// <summary>
        /// Saves changes to the name, color, and type asynchronously
        /// </summary>
        /// <example>This example shows how to modify the name, color, and type of an ROI, commit the change, and
        /// refresh the structure set:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "./credentials.json");
        /// var workspaceItem = await _proKnow.Workspaces.FindAsync(w => w.Name == "Clinical");
        /// var patientSummary = await _proKnow.Patients.FindAsync(workspaceItem.Id, p => p.Name == "Example");
        /// var patientItem = await patientSummary.GetAsync();
        /// var structureSetItem = patientItem.FindEntities(e => e.Type == "structure_set")[0];
        /// using (var draft = await structureSetItem.DraftAsync())
        /// {
        ///     var roiItem = draft.Rois.First(r => r.Name == "PTV");
        ///     await roiItem.DeleteAsync();
        ///     await draft.ApproveAsync();
        /// }
        /// await structureSetItem.RefreshAsync();
        /// </code>
        /// </example>
        public Task SaveAsync()
        {
            if (!IsEditable())
            {
                throw new InvalidOperationError("Item is not editable");
            }
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Lock", _structureSetItem.DraftLock.Id) };
            var properties = new Dictionary<string, object>() { { "name", Name }, { "color", Color }, { "type", Type } };
            var requestContent = new StringContent(JsonSerializer.Serialize(properties, _jsonSerializerOptions), Encoding.UTF8, "application/json");
            return _proKnow.Requestor.PutAsync($"/workspaces/{WorkspaceId}/structuresets/{_structureSetItem.Id}/draft/rois/{Id}", headerKeyValuePairs, requestContent);
        }

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="structureSetItem">The parent structure set</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId, StructureSetItem structureSetItem)
        {
            _proKnow = proKnow;
            _structureSetItem = structureSetItem;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new ColorJsonConverter());
            WorkspaceId = workspaceId;
        }
    }
}
