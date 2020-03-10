﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ProKnow.CustomMetric;

namespace ProKnow.Scorecard
{
    /// <summary>
    /// Interacts with scorecards in a ProKnow organization
    /// </summary>
    public class ScorecardTemplates
    {
        private ProKnow _proKnow;
        private IList<ScorecardTemplateSummary> _cache;

        /// <summary>
        /// Constructs a scorecard templates object
        /// </summary>
        /// <param name="proKnow">Parent ProKnow objet</param>
        public ScorecardTemplates(ProKnow proKnow)
        {
            _proKnow = proKnow;
            _cache = null;
        }

        /// <summary>
        /// Creates a scorecard template asynchronously
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetricNames">The ProKnow IDs for the custom metrics</param>
        /// <returns>The created scorecard template</returns>
        public async Task<ScorecardTemplateItem> CreateAsync(string name, IList<ComputedMetric> computedMetrics,
            IList<string> customMetricNames)
        {
            // Convert custom metric names to IDs
            var customMetrics = await Task.WhenAll(customMetricNames.Select(async (n) =>
                await _proKnow.CustomMetrics.ResolveByNameAsync(n)));
            var customMetricIds = new List<CustomMetricIdSchema>();
            foreach (var customMetric in customMetrics)
            {
                customMetricIds.Add(new CustomMetricIdSchema() { Id = customMetric.Id });
            }

            // Request the creation
            var requestSchema = new ScorecardTemplateCreateSchema()
                { Name = name, ComputedMetrics = computedMetrics, CustomMetricIdSchemas = customMetricIds };
            var jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
            var contentJson = JsonSerializer.Serialize(requestSchema, jsonSerializerOptions);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            string json = await _proKnow.Requestor.PostAsync("/metrics/templates", null, content);
            _cache = null;

            // Return the created scorecard template
            var responseSchema = JsonSerializer.Deserialize<ScorecardTemplateCreateSchema>(json);
            return new ScorecardTemplateItem(_proKnow, responseSchema.Id, name, computedMetrics, customMetrics);
        }

        /// <summary>
        /// Deletes a scorecard template asynchronously
        /// </summary>
        /// <param name="scorecardTemplateId">The ProKnow ID for the scorecard template</param>
        public async Task DeleteAsync(string scorecardTemplateId)
        {
            await _proKnow.Requestor.DeleteAsync($"/metrics/templates/{scorecardTemplateId}");
            _cache = null;
        }

        /// <summary>
        /// Finds a scorecard template asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first scorecard template that satisfies the predicate or null if the predicate was null or no
        /// scorecard template satisfies the predicate</returns>
        public async Task<ScorecardTemplateSummary> FindAsync(Func<ScorecardTemplateSummary, bool> predicate)
        {
            if (_cache == null)
            {
                await QueryAsync();
            }
            return Find(predicate);
        }

        /// <summary>
        /// Gets a scorecard template item asynchronously
        /// </summary>
        /// <param name="scorecardTemplateId">The ProKnow ID for the scorecard template</param>
        /// <returns>The scorecard template item</returns>
        public async Task<ScorecardTemplateItem> GetAsync(string scorecardTemplateId)
        {
            var json = await _proKnow.Requestor.GetAsync($"/metrics/templates/{scorecardTemplateId}");
            return new ScorecardTemplateItem(_proKnow, json);
        }

        /// <summary>
        /// Queries for scorecard templates asynchronously
        /// </summary>
        /// <returns>The scorecard templates</returns>
        public async Task<IList<ScorecardTemplateSummary>> QueryAsync()
        {
            string scorecardTemplatesJson = await _proKnow.Requestor.GetAsync("/metrics/templates");
            return DeserializeScorecardTemplates(scorecardTemplatesJson);
        }

        /// <summary>
        /// Resolves a scorecard template asynchronously
        /// </summary>
        /// <param name="scorecardTemplate">The ProKnow ID or name of the scorecard template</param>
        /// <returns>The scorecard template corresponding to the specified ID or name or null if no matching
        /// scorecard template was found</returns>
        public Task<ScorecardTemplateSummary> ResolveAsync(string scorecardTemplate)
        {
            Regex regex = new Regex(@"^[0-9a-f]{32}$");
            Match match = regex.Match(scorecardTemplate);
            if (match.Success)
            {
                return ResolveByIdAsync(scorecardTemplate);
            }
            else
            {
                return ResolveByNameAsync(scorecardTemplate);
            }
        }

        /// <summary>
        /// Resolves a scorecard template by its ProKnow ID asynchronously
        /// </summary>
        /// <param name="scorecardTemplateId">The ProKnow ID of the scorecard template</param>
        /// <returns>The scorecard template corresponding to the specified ID or null if no matching scorecard template
        /// was found</returns>
        public Task<ScorecardTemplateSummary> ResolveByIdAsync(string scorecardTemplateId)
        {
            if (String.IsNullOrWhiteSpace(scorecardTemplateId))
            {
                throw new ArgumentException("The scorecard template ID must be specified.");
            }
            return FindAsync(t => t.Id == scorecardTemplateId);
        }

        /// <summary>
        /// Resolves a scorecard template by its name asynchronously
        /// </summary>
        /// <param name="scorecardTemplateName">The name of the scorecard template</param>
        /// <returns>The scorecard template corresponding to the specified name or null if no matching scorecard template
        /// was found</returns>
        public Task<ScorecardTemplateSummary> ResolveByNameAsync(string scorecardTemplateName)
        {
            if (String.IsNullOrWhiteSpace(scorecardTemplateName))
            {
                throw new ArgumentException("The scorecard template name must be specified.");
            }
            return FindAsync(t => t.Name == scorecardTemplateName);
        }

        /// <summary>
        /// Finds a scorecard template based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first scorecard template that satisfies the predicate or null if the predicate was null or no
        /// scorecard template satisfies the predicate were found</returns>
        private ScorecardTemplateSummary Find(Func<ScorecardTemplateSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            foreach (var scorecardTemplate in _cache)
            {
                if (predicate(scorecardTemplate))
                {
                    return scorecardTemplate;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a collection of scorecard templates from their JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the scorecard templates</param>
        /// <returns>A collection of scorecard templates</returns>
        private IList<ScorecardTemplateSummary> DeserializeScorecardTemplates(string json)
        {
            var scorecardTemplates = JsonSerializer.Deserialize<IList<ScorecardTemplateSummary>>(json);
            foreach (var scorecardTemplate in scorecardTemplates)
            {
                scorecardTemplate.PostProcessDeserialization(_proKnow);
            }
            _cache = scorecardTemplates;
            return _cache;
        }
    }
}