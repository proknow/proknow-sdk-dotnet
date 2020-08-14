using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// A container for the structure set entity data
    /// </summary>
    public class StructureSetData
    {
        /// <summary>
        /// The version ID
        /// </summary>
        [JsonPropertyName("version")]
        public string VersionId { get; set; }

        /// <summary>
        /// The label
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The ROIs
        /// </summary>
        [JsonPropertyName("rois")]
        public StructureSetRoiItem[] Rois { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
