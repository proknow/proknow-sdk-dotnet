using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Represents a custom metric
    /// </summary>
    [JsonConverter(typeof(CustomMetricItemJsonConverter))]
    public class CustomMetricItem
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
        /// The context
        /// </summary>
        [JsonPropertyName("context")]
        public string Context { get; set; }

        /// <summary>
        /// The type
        /// </summary>
        [JsonPropertyName("type")]
        public CustomMetricType Type { get; set; }

        /// <summary>
        /// The objectives or null if not specified
        /// </summary>
        [JsonPropertyName("objectives")]
        public IList<MetricBin> Objectives { get; set; }

        /// <summary>
        /// Deletes this custom metric asynchronously
        /// </summary>
        public Task DeleteAsync()
        {
            return _proKnow.CustomMetrics.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves name and context changes to this instance asynchronously
        /// </summary>
        public Task SaveAsync()
        {
            var customMetricItem = new CustomMetricItem()
            {
                Name = Name,
                Context = Context
            };
            string requestJson = JsonSerializer.Serialize(customMetricItem);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            return _proKnow.Requestor.PutAsync($"/metrics/custom/{Id}", null, content);
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
        /// Provide a copy of this instance containing only the information required to represent it in a scorecard
        /// create or save request
        /// </summary>
        /// <returns>A copy of this instance containing only the information required to represent it in a scorecard
        /// create or save request</returns>
        internal CustomMetricItem ConvertToScorecardSchema()
        {
            return new CustomMetricItem()
            {
                Id = Id,
                Objectives = Objectives
            };
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal void PostProcessSerialization(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }
    }
}
