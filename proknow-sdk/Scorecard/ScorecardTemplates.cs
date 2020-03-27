using System;
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
    /// Interacts with scorecard templates in a ProKnow organization
    /// </summary>
    public class ScorecardTemplates
    {
        private ProKnow _proKnow;
        private IList<ScorecardTemplateSummary> _cache;

        /// <summary>
        /// Constructs a scorecard templates object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal ScorecardTemplates(ProKnow proKnow)
        {
            _proKnow = proKnow;
            _cache = null;
        }

        /// <summary>
        /// Creates a scorecard template asynchronously
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics</param>
        /// <returns>The created scorecard template</returns>
        public async Task<ScorecardTemplateItem> CreateAsync(string name, IList<ComputedMetric> computedMetrics,
            IList<CustomMetric> customMetrics)
        {
            // Check arguments
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (computedMetrics == null)
            {
                throw new ArgumentNullException("computedMetrics");
            }
            if (customMetrics == null)
            {
                throw new ArgumentNullException("customMetrics");
            }

            // Resolve custom metrics (obtain their IDs) and add objectives
            var resolvedCustomMetrics = new List<CustomMetricItem>();
            var tasks = new List<Task>();
            foreach (var inputCustomMetric in customMetrics)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var resolvedCustomMetric = await _proKnow.CustomMetrics.ResolveByNameAsync(inputCustomMetric.Name);
                    resolvedCustomMetric.Objectives = inputCustomMetric.Objectives;
                    resolvedCustomMetrics.Add(resolvedCustomMetric);
                }));
            }
            await Task.WhenAll(tasks);

            // Convert custom metrics to their scorecard template creation schema
            var customMetricIdsAndObjectives = resolvedCustomMetrics.Select(c => c.ConvertToScorecardSchema()).ToList();

            // Request the creation
            var requestSchema = new ScorecardTemplateItem(null, null, name, computedMetrics, customMetricIdsAndObjectives);
            var contentJson = JsonSerializer.Serialize(requestSchema);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            string responseJson = await _proKnow.Requestor.PostAsync("/metrics/templates", null, content);
            _cache = null;

            // Return the created scorecard template, with complete custom metric representations
            var responseSchema = JsonSerializer.Deserialize<ScorecardTemplateItem>(responseJson);
            return new ScorecardTemplateItem(_proKnow, responseSchema.Id, responseSchema.Name,
                responseSchema.ComputedMetrics, resolvedCustomMetrics);
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
