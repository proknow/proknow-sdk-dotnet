using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.CustomMetric
{
    /// <summary>
    /// Interacts with the custom metrics in a ProKnow organization
    /// </summary>
    public class CustomMetrics
    {
        private ProKnow _proKnow;
        private IList<CustomMetricItem> _cache;

        /// <summary>
        /// Creates a custom metrics object
        /// </summary>
        /// <param name="proKnow">The parent ProKnow object</param>
        public CustomMetrics(ProKnow proKnow)
        {
            _proKnow = proKnow;
        }

        /// <summary>
        /// Creates a custom metric asynchronously
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="context">The context</param>
        /// <param name="type">The type</param>
        /// <param name="enumValues">The enum values if type is enum</param>
        /// <returns>The created custom metric</returns>
        public async Task<CustomMetricItem> CreateAsync(string name, string context, string type, string[] enumValues = null)
        {
            var customMetricItem = new CustomMetricItem(name, context, new CustomMetricType(type, enumValues));
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var content = new StringContent(JsonSerializer.Serialize(customMetricItem, jsonSerializerOptions),
                Encoding.UTF8, "application/json");
            string customMetricJson = await _proKnow.Requestor.PostAsync("/metrics/custom", null, content);
            _cache = null;
            customMetricItem = DeserializeCustomMetric(customMetricJson);
            return customMetricItem;
        }

        /// <summary>
        /// Deletes a custom metric asynchronously
        /// </summary>
        /// <param name="customMetricId">The ProKnow ID for the custom metric</param>
        public async Task DeleteAsync(string customMetricId)
        {
            await _proKnow.Requestor.DeleteAsync($"/metrics/custom/{customMetricId}");
            _cache = null;
        }

        /// <summary>
        /// Queries for custom metrics asynchronously
        /// </summary>
        /// <returns>The custom metrics</returns>
        public async Task<IList<CustomMetricItem>> QueryAsync()
        {
            string customMetricsJson = await _proKnow.Requestor.GetAsync("/metrics/custom");
            return DeserializeCustomMetrics(customMetricsJson);
        }

        /// <summary>
        /// Creates a collection of custom metric items from their JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the custom metric items</param>
        /// <returns>A collection of custom metric items</returns>
        private IList<CustomMetricItem> DeserializeCustomMetrics(string json)
        {
            var customMetricItems = JsonSerializer.Deserialize<IList<CustomMetricItem>>(json);
            foreach (var customMetricItem in customMetricItems)
            {
                customMetricItem.PostProcessDeserialization(this, this._proKnow.Requestor);
            }
            _cache = customMetricItems;
            return _cache.ToList();
        }

        /// <summary>
        /// Creates a custom metric item from its JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the custom metric item</param>
        /// <returns>A custom metric item</returns>
        private CustomMetricItem DeserializeCustomMetric(string json)
        {
            var customMetricItem = JsonSerializer.Deserialize<CustomMetricItem>(json);
            customMetricItem.PostProcessDeserialization(this, this._proKnow.Requestor);
            return customMetricItem;
        }
    }
}
