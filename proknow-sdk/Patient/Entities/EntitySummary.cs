using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Provides a summary of an entity for a study
    /// </summary>
    public class EntitySummary
    {
        static private Dictionary<string, string> typeToRoutePartMap = new Dictionary<string, string>() {
            { "image_set", "imagesets"},
            { "structure_set", "structuresets" },
            { "plan", "plans" },
            { "dose", "doses" }
        };
        private const int RETRY_DELAY = 200;
        private const int MAX_TOTAL_RETRY_DELAY = 5000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

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
        /// The entity type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// All entity summary attributes
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// Entities within this entity
        /// </summary>
        [JsonPropertyName("entities")]
        public IList<EntitySummary> Entities { get; set; }

        /// <summary>
        /// Determines whether this object satisfies a predicate and/or specified property values
        /// </summary>
        /// <param name="predicate">The optional predicate</param>
        /// <param name="properties">Optional properties</param>
        /// <returns>True if this object satisfies the predicate (if specified) and all property filters (if specified); otherwise false</returns>
        public virtual bool DoesMatch(Func<EntitySummary, bool> predicate = null, params KeyValuePair<string, object>[] properties)
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
        /// Gets the corresponding entity item asynchronously
        /// </summary>
        /// <returns>The corresponding entity item</returns>
        public async Task<EntityItem> GetAsync()
        {
            var numberOfRetries = 0;
            while (true)
            {
                var entityItem = await GetEntityItemAsync();
                if (entityItem.Status == "completed")
                {
                    return entityItem;
                }
                else
                {
                    if (numberOfRetries < MAX_RETRIES)
                    {
                        await Task.Delay(RETRY_DELAY);
                        numberOfRetries++;
                    }
                    else
                    {
                        throw new TimeoutException($"Timeout while waiting for {Type} entity to reach completed status.");
                    }
                }
            }
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
        internal void PostProcessDeserialization(Requestor requestor, string workspaceId, string patientId)
        {
            _requestor = requestor;
            WorkspaceId = workspaceId;
            PatientId = patientId;

            // Post-process deserialization of entities
            foreach (var entity in Entities)
            {
                entity.PostProcessDeserialization(_requestor, WorkspaceId, PatientId);
            }
        }

        /// <summary>
        /// Gets the corresponding entity item asynchronously
        /// </summary>
        /// <returns></returns>
        private Task<EntityItem> GetEntityItemAsync()
        {
            if (!typeToRoutePartMap.ContainsKey(Type))
            {
                throw new ArgumentOutOfRangeException("The entity 'type' must be one of 'image_set', 'structure_set', 'plan', or 'dose'.");
            }
            var entityRoutePart = typeToRoutePartMap[Type];
            var entityJsonTask = _requestor.GetAsync($"/workspaces/{WorkspaceId}/{entityRoutePart}/{Id}");
            return entityJsonTask.ContinueWith(t => DeserializeEntity(t.Result));
        }

        /// <summary>
        /// Creates an entity item from its JSON representation
        /// </summary>
        /// <param name="json">JSON representation of the entity item</param>
        /// <returns>An entity item</returns>
        private EntityItem DeserializeEntity(string json)
        {
            EntityItem entityItem;
            switch (Type)
            {
                case "image_set":
                    entityItem = JsonSerializer.Deserialize<ImageSetItem>(json);
                    break;
                case "structure_set":
                    entityItem = JsonSerializer.Deserialize<StructureSetItem>(json);
                    break;
                case "plan":
                    entityItem = JsonSerializer.Deserialize<PlanItem>(json);
                    break;
                case "dose":
                    entityItem = JsonSerializer.Deserialize<DoseItem>(json);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The entity 'type' must be one of 'image_set', 'structure_set', 'plan', or 'dose'.");
            }
            entityItem.PostProcessDeserialization(_requestor, WorkspaceId, PatientId);
            return entityItem;
        }
    }
}
