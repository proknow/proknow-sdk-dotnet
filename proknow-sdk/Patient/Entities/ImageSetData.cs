using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// A container for the image set entity data
    /// </summary>
    public class ImageSetData
    {
        /// <summary>
        /// The images
        /// </summary>
        [JsonPropertyName("images")]
        public IList<Image> Images { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
