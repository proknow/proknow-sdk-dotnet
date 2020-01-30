using System;
using System.Collections.Generic;
using System.Text;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a structure set for a patient
    /// </summary>
    public class StructureSetItem : EntityItem
    {

        /// <summary>
        /// Determines whether this object satisfies a predicate and/or specified property values
        /// </summary>
        /// <param name="predicate">The optional predicate</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>True if this object satisfies the predicate (if specified) and all property filters (if specified); otherwise false</returns>
        public override bool DoesMatch(Func<EntityItem, bool> predicate = null, params KeyValuePair<string, object>[] properties)
        {
            return base.DoesMatch(predicate, properties);

            //todo--also handle ROIs and Versions properties
        }
    }
}
