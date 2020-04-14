﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Provides a summary of a scorecard template
    /// </summary>
    public class ScorecardTemplateSummary
    {
        /// <summary>
        /// Root object for interfacing with the ProKnow API
        /// </summary>
        protected ProKnowApi _proKnow;

        /// <summary>
        /// The ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create a scorecard template summary
        /// </summary>
        public ScorecardTemplateSummary()
        {
        }

        /// <summary>
        /// Gets the full representation of the scorecard template asynchronously
        /// </summary>
        /// <returns>The full representation of a scorecard template</returns>
        public virtual Task<ScorecardTemplateItem> GetAsync()
        {
            return _proKnow.ScorecardTemplates.GetAsync(Id);
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }
    }
}
