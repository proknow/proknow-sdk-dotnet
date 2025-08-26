using System;

namespace ProKnow.Patient
{
    /// <summary>
    /// Represents criteria for querying patients list. Criteria are logically ANDed together.
    /// </summary>
    public class PatientsQueryCriteria
    {
        /// <summary>
        /// Patient ID or Name. Partial matches are allowed. Search is case-insensitive.
        /// </summary>
        public string Patient { get; set; }

        /// <summary>
        /// Structure Name. Search is case-insensitive.
        /// </summary>
        public string Structure { get; set; }
    }
}
