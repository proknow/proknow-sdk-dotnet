using ProKnow.Scorecard;
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
    /// Represents a patient scorecard
    /// </summary>
    public class PatientScorecardItem : ScorecardTemplateItem
    {
        private string _workspaceId;
        private string _patientId;
        private PatientScorecards _patientScorecards;

        /// <summary>
        /// Used by deserialization to create patient scorecard item
        /// </summary>
        public PatientScorecardItem() : base()
        {
        }

        /// <summary>
        /// Creates an patient scorecard item
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="patientId">The patient ProKnow ID</param>
        /// <param name="patientScorecards">Interacts with scorecards for an patient in a ProKnow organization</param>
        /// <param name="id">The patient scorecard ProKnow ID</param>
        /// <param name="name">The name</param>
        /// <param name="computedMetrics">The computed metrics</param>
        /// <param name="customMetrics">The custom metrics</param>
        internal PatientScorecardItem(ProKnowApi proKnow, string workspaceId, string patientId,
            PatientScorecards patientScorecards, string id, string name, IList<ComputedMetric> computedMetrics,
            IList<CustomMetricItem> customMetrics) : base(proKnow, id, name, workspaceId, computedMetrics, customMetrics)
        {
            _workspaceId = workspaceId;
            _patientId = patientId;
            _patientScorecards = patientScorecards;
        }

        /// <summary>
        /// Creates a patient scorecard item from its JSON representation
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="patientId">The patient ProKnow ID</param>
        /// <param name="patientScorecards">Interacts with scorecards for an patient in a ProKnow organization</param>
        /// <param name="json">JSON representation of the patient scorecard item</param>
        internal PatientScorecardItem(ProKnowApi proKnow, string workspaceId, string patientId,
            PatientScorecards patientScorecards, string json) : base(proKnow, json)
        {
            _workspaceId = workspaceId;
            _patientId = patientId;
            _patientScorecards = patientScorecards;
        }

        /// <summary>
        /// Deletes this patient scorecard item instance asynchronously
        /// </summary>
        public override async Task DeleteAsync()
        {
            await _patientScorecards.DeleteAsync(Id);
        }

        /// <summary>
        /// Saves changes to an patient scorecard asynchronously
        /// </summary>
        //TODO--Add example to update a custom metric value for a scorecard
        public override async Task SaveAsync()
        {
            var route = $"/workspaces/{_workspaceId}/patients/{_patientId}/metrics/sets/{Id}";
            var jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            var contentJson = JsonSerializer.Serialize(ConvertToSaveSchema(), jsonSerializerOptions);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync(route, null, content);
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
        internal override ScorecardTemplateItem ConvertToSaveSchema()
        {
            return new PatientScorecardItem()
            {
                Name = Name,
                ComputedMetrics = ComputedMetrics,
                CustomMetrics = CustomMetrics.Select(c => c.ConvertToScorecardSchema()).ToList()
            };
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ProKnow ID</param>
        /// <param name="patientId">The patient ProKnow ID</param>
        /// <param name="patientScorecards">Interacts with scorecards for a patient in a ProKnow organization</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId, string patientId,
            PatientScorecards patientScorecards)
        {
            _proKnow = proKnow;
            _workspaceId = workspaceId;
            _patientId = patientId;
            _patientScorecards = patientScorecards;
        }
    }
}
