﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// The base class for entities in a ProKnow patient
    /// </summary>
    public class EntityItem
    {
        private Requestor _requestor;

        /// <summary>
        /// The patient workspace ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonIgnore]
        public string PatientId { get; internal set; }

        /// <summary>
        /// The entity ProKnow ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// All entity attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// The entity description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The entity metadata (custom metrics)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        //todo--Scorecards

        /// <summary>
        /// Determines whether this object satisfies a predicate and/or specified property values
        /// </summary>
        /// <param name="predicate">The optional predicate</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>True if this object satisfies the predicate (if specified) and all property filters (if specified); otherwise false</returns>
        public virtual bool DoesMatch(Func<EntityItem, bool> predicate = null, params KeyValuePair<string, object>[] properties)
        {
            if (predicate != null && !predicate(this))
            {
                return false;
            }
            foreach (var kvp in properties)
            {
                switch (kvp.Key)
                {
                    case "id":
                        if (!Id.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "description":
                        if (!Description.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    case "metadata":
                        if (!Metadata.Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                    default:
                        if (!Data.ContainsKey(kvp.Key) || !Data[kvp.Key].Equals(kvp.Value))
                        {
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="requestor">Issues requests to the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        internal virtual void PostProcessDeserialization(Requestor requestor, string workspaceId, string patientId)
        {
            _requestor = requestor;
            WorkspaceId = workspaceId;
            PatientId = patientId;
        }
    }
}
