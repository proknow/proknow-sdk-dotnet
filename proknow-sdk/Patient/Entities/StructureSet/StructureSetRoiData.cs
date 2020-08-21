using ProKnow.Exceptions;
using ProKnow.Geometry;
using ProKnow.JsonConverters;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents the contour and point data for an ROI
    /// </summary>
    public class StructureSetRoiData
    {
        private ProKnowApi _proKnow;
        private string _workspaceId;
        private StructureSetItem _structureSetItem;
        private StructureSetRoiItem _structureSetRoiItem;
        private JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// The contours
        /// </summary>
        [JsonPropertyName("contours")]
        public StructureSetRoiContour[] Contours { get; set; }

        /// <summary>
        /// The points
        /// </summary>
        [JsonPropertyName("points")]
        [JsonConverter(typeof(Points3DJsonConverter))]
        public Point3D[] Points { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Indicates whether this ROI contour and point data is editable
        /// </summary>
        /// <returns>True if this ROI contour and point data is editable; otherwise false</returns>
        public bool IsEditable()
        {
            return _structureSetItem.IsEditable;
        }

        /// <summary>
        /// Saves this ROI contour and point data asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            if (!IsEditable())
            {
                throw new InvalidOperationError("Item is not editable");
            }
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Lock", _structureSetItem.DraftLock.Id) };
            var properties = new Dictionary<string, object>() { { "version", 2 }, { "contours", Contours }, { "lines", new Point2D[0][] }, { "points", Points } };
            var content = JsonSerializer.Serialize(properties, _jsonSerializerOptions);
            var requestContent = new StringContent(content, Encoding.UTF8, "application/json");
            var route = $"/workspaces/{_workspaceId}/structuresets/{_structureSetItem.Id}/draft/rois/{_structureSetRoiItem.Id}/data";
            var response = await _proKnow.Requestor.PutAsync(route, headerKeyValuePairs, requestContent);
            var structureSetRoiData = JsonSerializer.Deserialize<StructureSetRoiData>(response);
            _structureSetRoiItem.Tag = structureSetRoiData.ExtensionData["tag"].ToString();
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="structureSetItem">The structure set to which this data belongs</param>
        /// <param name="structureSetRoiItem">The ROI to which this data belongs</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, StructureSetItem structureSetItem, StructureSetRoiItem structureSetRoiItem)
        {
            _proKnow = proKnow;
            _workspaceId = structureSetItem.WorkspaceId;
            _structureSetItem = structureSetItem;
            _structureSetRoiItem = structureSetRoiItem;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new Points3DJsonConverter());
        }
    }
}
