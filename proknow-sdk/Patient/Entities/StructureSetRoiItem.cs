using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a region of interest (ROI) in a structure set
    /// </summary>
    public class StructureSetRoiItem
    {
        private ProKnowApi _proKnow;
        private StructureSetItem _structureSetItem;

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
        public int[] Color { get; set; }

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
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        //todo--Implement DeleteAsync method

        //todo--Implement GetDataAsync method

        //todo--Implement IsEditable method

        //todo--Implement SaveAsync method

        /// <summary>
        /// Provides a string representation of this object
        /// </summary>
        /// <returns>A string representation of this object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Constructs a StructureSetRoiItem
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="structureSetItem">The parent structure set</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId, StructureSetItem structureSetItem)
        {
            _proKnow = proKnow;
            _structureSetItem = structureSetItem;
            WorkspaceId = workspaceId;
        }
    }
}
