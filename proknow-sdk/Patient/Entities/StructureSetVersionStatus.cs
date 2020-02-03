using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a structure set version status returned by the ProKnow API
    /// </summary>
    public class StructureSetVersionStatus
    {

        /// <summary>
        /// The entity processing status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
