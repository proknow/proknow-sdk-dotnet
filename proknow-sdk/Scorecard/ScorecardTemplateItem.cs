using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using ProKnow.CustomMetric;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Represents a scorecard template
    /// </summary>
    public class ScorecardTemplateItem
    {
        private ProKnow _proKnow;

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
        /// The computed metrics
        /// </summary>
        [JsonPropertyName("computed")]
        public IList<ComputedMetric> ComputedMetrics { get; set; }

        /// <summary>
        /// The custom metrics
        /// </summary>
        [JsonPropertyName("custom")]
        public IList<CustomMetricItem> CustomMetrics { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Used by deserialization to create scorecard template item
        /// </summary>
        protected ScorecardTemplateItem()
        {
        }

        /// <summary>
        /// Creates a scorecard template
        /// </summary>
        /// <param name="proKnow">The parent ProKnow object</param>
        /// <param name="id">The ProKnow ID</param>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics</param>
        internal ScorecardTemplateItem(ProKnow proKnow, string id, string name, IList<ComputedMetric> computedMetrics,
            IList<CustomMetricItem> customMetrics)
        {
            _proKnow = proKnow;
            Id = id;
            Name = name;
            ComputedMetrics = computedMetrics;
            CustomMetrics = customMetrics;
        }

        /// <summary>
        /// Creates a scorecard template item from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="json">JSON representation of the scorecard template item</param>
        internal ScorecardTemplateItem(ProKnow proKnow, string json)
        {
            var scorecardTemplateItem = JsonSerializer.Deserialize<ScorecardTemplateItem>(json);
            _proKnow = proKnow;
            Id = scorecardTemplateItem.Id;
            Name = scorecardTemplateItem.Name;
            ComputedMetrics = scorecardTemplateItem.ComputedMetrics;
            CustomMetrics = scorecardTemplateItem.CustomMetrics;
        }

        /// <summary>
        /// Deletes this scorecard template instance asynchronously
        /// </summary>
        public async Task DeleteAsync()
        {
            await _proKnow.ScorecardTemplates.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves changes to a scorecard template asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var contentJson = JsonSerializer.Serialize(ConvertToSaveSchema(), jsonSerializerOptions);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/metrics/templates/{Id}", null, content);
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
        /// Provide a copy of this instance containing only the information required to represent it in a save request
        /// </summary>
        /// <returns>A copy of this instance containing only the information required to represent it in a save
        /// request</returns>
        internal ScorecardTemplateItem ConvertToSaveSchema()
        {
            return new ScorecardTemplateItem()
            {
                Name = Name,
                ComputedMetrics = ComputedMetrics,
                CustomMetrics = CustomMetrics.Select(c => c.ConvertToScorecardTemplateSchema()).ToList()
            };
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessDeserialization(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }
    }
}
