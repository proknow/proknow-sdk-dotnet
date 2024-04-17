using ProKnow.Scorecard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient
{
    /// <summary>
    /// Interacts with scorecards for a patient in a ProKnow organization
    /// </summary>
    public class PatientScorecards
    {
        private readonly ProKnowApi _proKnow;
        private readonly string _workspaceId;
        private readonly string _patientId;

        /// <summary>
        /// Constructs an PatientScorecards object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="patientId">The ProKnow ID for the patient</param>
        public PatientScorecards(ProKnowApi proKnow, string workspaceId, string patientId)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _patientId = patientId;
        }

        /// <summary>
        /// Creates a patient scorecard asynchronously
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics</param>
        /// <returns>The created patient scorecard</returns>
        public async Task<PatientScorecardItem> CreateAsync(string name, IList<ComputedMetric> computedMetrics,
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
            var route = $"/workspaces/{_workspaceId}/patients/{_patientId}/metrics/sets";
            var requestSchema = new PatientScorecardItem(null, null, null, null, null, name, computedMetrics, customMetricIdsAndObjectives);
            var jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            var contentJson = JsonSerializer.Serialize(requestSchema, jsonSerializerOptions);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            string responseJson = await _proKnow.Requestor.PostAsync(route, null, content);

            // Return the created patient scorecard, with complete custom metric representations
            var responseSchema = JsonSerializer.Deserialize<ScorecardTemplateItem>(responseJson);
            return new PatientScorecardItem(_proKnow, _workspaceId, _patientId, this, responseSchema.Id, responseSchema.Name,
                responseSchema.ComputedMetrics, resolvedCustomMetrics);
        }

        /// <summary>
        /// Deletes a patient scorecard asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID of the patient scorecard</param>
        public async Task DeleteAsync(string id)
        {
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{_workspaceId}/patients/{_patientId}/metrics/sets/{id}");
        }

        /// <summary>
        /// Finds a patient scorecard asynchronously based on a predicate
        /// </summary>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first patient scorecard that satisfies the predicate or null if the predicate was null or no
        /// patient scorecard satisfies the predicate</returns>
        public async Task<PatientScorecardSummary> FindAsync(Func<PatientScorecardSummary, bool> predicate)
        {
            var patientScorecards = await QueryAsync();
            return Find(patientScorecards, predicate);
        }

        /// <summary>
        /// Gets a patient scorecard item asynchronously
        /// </summary>
        /// <param name="id">The ProKnow ID for the patient scorecard</param>
        /// <returns>The patient scorecard item</returns>
        public async Task<PatientScorecardItem> GetAsync(string id)
        {
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{_workspaceId}/patients/{_patientId}/metrics/sets/{id}");
            return new PatientScorecardItem(_proKnow, _workspaceId, _patientId, this, json);
        }

        /// <summary>
        /// Queries for patient scorecards asynchronously
        /// </summary>
        /// <returns>The patient scorecards</returns>
        public async Task<IList<PatientScorecardSummary>> QueryAsync()
        {
            string json = await _proKnow.Requestor.GetAsync($"/workspaces/{_workspaceId}/patients/{_patientId}/metrics/sets");
            return DeserializePatientScorecards(json);
        }

        /// <summary>
        /// Finds an patient scorecard based on a predicate
        /// </summary>
        /// <param name="patientScorecards">The patient scorecards to search</param>
        /// <param name="predicate">The predicate for the search</param>
        /// <returns>The first patient scorecard that satisfies the predicate or null if the predicate was null or no
        /// patient scorecard that satisfies the predicate was found</returns>
        private PatientScorecardSummary Find(IList<PatientScorecardSummary> patientScorecards,
            Func<PatientScorecardSummary, bool> predicate)
        {
            if (predicate == null)
            {
                return null;
            }
            foreach (var patientScorecard in patientScorecards)
            {
                if (predicate(patientScorecard))
                {
                    return patientScorecard;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a collection of patient scorecards from their JSON representation
        /// </summary>
        /// <param name="json">The JSON representation of the patient scorecards</param>
        /// <returns>A collection of patient scorecards</returns>
        private IList<PatientScorecardSummary> DeserializePatientScorecards(string json)
        {
            var patientScorecards = JsonSerializer.Deserialize<IList<PatientScorecardSummary>>(json);
            foreach (var patientScorecard in patientScorecards)
            {
                patientScorecard.PostProcessDeserialization(_proKnow, this);
            }
            return patientScorecards;
        }
    }
}
