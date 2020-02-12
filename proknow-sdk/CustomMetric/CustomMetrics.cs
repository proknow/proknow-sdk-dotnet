using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    }
}
