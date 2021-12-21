using ProKnow.Scorecard;
using System.Threading.Tasks;

namespace ProKnow.Patient
{
    /// <summary>
    /// Provides a summary of a patient scorecard
    /// </summary>
    public class PatientScorecardSummary : ScorecardTemplateSummary
    {
        private PatientScorecards _patientScorecards;

        /// <summary>
        /// Used by deserialization to create a patient scorecard summary
        /// </summary>
        public PatientScorecardSummary() : base()
        {
        }

        /// <summary>
        /// Gets the full representation of the patient scorecard asynchronously
        /// </summary>
        /// <returns>The full representation of a patient scorecard</returns>
        public override Task<ScorecardTemplateItem> GetAsync()
        {
            return ConvertToBaseTask(_patientScorecards.GetAsync(Id));
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
        /// <param name="patientScorecards">Interacts with scorecards for a patient in a ProKnow organization</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, PatientScorecards patientScorecards)
        {
            _proKnow = proKnow;
            _patientScorecards = patientScorecards;
        }

        /// <summary>
        /// Helper to cast PatientScorecardItem task to ScorecardTemplateItem task
        /// </summary>
        /// <param name="task">The PatientScorecardItem task</param>
        /// <returns>A ScorecardTemplateItem task</returns>
        private static async Task<ScorecardTemplateItem> ConvertToBaseTask(Task<PatientScorecardItem> task)
        {
            var PatientScorecardItem = await task;
            return PatientScorecardItem as ScorecardTemplateItem;
        }
    }
}
