﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Interacts with the custom metrics in a ProKnow organization
    /// </summary>
    public class CustomMetrics
    {
        private readonly ProKnowApi _proKnow;
        private IList<CustomMetricItem> _cache;

        /// <summary>
        /// Creates a custom metrics object
        /// </summary>
        /// <param name="proKnow">The parent ProKnow object</param>
        internal CustomMetrics(ProKnowApi proKnow)
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
            var customMetricItem = new CustomMetricItem()
            {
                Name = name,
                Context = context,
                Type = new CustomMetricType(type, enumValues)
            };
            string requestJson = JsonSerializer.Serialize(customMetricItem);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            string responseJson = await _proKnow.Requestor.PostAsync("/metrics/custom", null, content);
            _cache = null;
            customMetricItem = JsonSerializer.Deserialize<CustomMetricItem>(responseJson);
            customMetricItem.PostProcessSerialization(_proKnow);
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
        /// Finds a custom metric item asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first custom metric item that satisfies the predicate or null if the predicate was null or no
        /// custom metric item satisfies the predicate</returns>
        public async Task<CustomMetricItem> FindAsync(Func<CustomMetricItem, bool> predicate)
        {
            if (_cache == null)
            {
                await QueryAsync();
            }
            return Find(predicate);
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
        /// Resolves a custom metric asynchronously
        /// </summary>
        /// <param name="customMetric">The ProKnow ID or name of the custom metric</param>
        /// <returns>The custom metric item corresponding to the specified ID or name or null if no matching
        /// custom metric was found</returns>
        public Task<CustomMetricItem> ResolveAsync(string customMetric)
        {
            Regex regex = new Regex(@"^[0-9a-f]{32}$");
            Match match = regex.Match(customMetric);
            if (match.Success)
            {
                return ResolveByIdAsync(customMetric);
            }
            else
            {
                return ResolveByNameAsync(customMetric);
            }
        }

        /// <summary>
        /// Resolves a custom metric by its ProKnow ID asynchronously
        /// </summary>
        /// <param name="customMetricId">The ProKnow ID of the custom metric</param>
        /// <returns>The custom metric item corresponding to the specified ID or null if no matching custom metric
        /// was found</returns>
        public Task<CustomMetricItem> ResolveByIdAsync(string customMetricId)
        {
            if (String.IsNullOrWhiteSpace(customMetricId))
            {
                throw new ArgumentException("The custom metric ID must be specified.");
            }
            return FindAsync(t => t.Id == customMetricId);
        }

        /// <summary>
        /// Resolves a custom metric by its name asynchronously
        /// </summary>
        /// <param name="customMetricName">The name of the custom metric</param>
        /// <returns>The custom metric item corresponding to the specified name or null if no matching custom metric
        /// was found</returns>
        public Task<CustomMetricItem> ResolveByNameAsync(string customMetricName)
        {
            if (String.IsNullOrWhiteSpace(customMetricName))
            {
                throw new ArgumentException("The custom metric name must be specified.");
            }
            return FindAsync(t => t.Name == customMetricName);
        }

        /// <summary>
        /// Finds a custom metric item based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first custom metric item that satisfies the predicate or null if the predicate was null or no
        /// custom metric item satisfies the predicate were found</returns>
        private CustomMetricItem Find(Func<CustomMetricItem, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            foreach (var customMetricItem in _cache)
            {
                if (predicate(customMetricItem))
                {
                    return customMetricItem;
                }
            }
            return null;
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
                customMetricItem.PostProcessSerialization(_proKnow);
            }
            _cache = customMetricItems;
            return _cache.ToList();
        }
    }
}
