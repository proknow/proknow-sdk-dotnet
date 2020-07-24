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
        private static readonly Dictionary<string, string> typeToRoutePartMap = new Dictionary<string, string>() {
            { "image_set", "imagesets"},
            { "structure_set", "structuresets" },
            { "plan", "plans" },
            { "dose", "doses" }
        };
        private const int RETRY_DELAY = 200;
        private const int MAX_TOTAL_RETRY_DELAY = 5000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

        private ProKnowApi _proKnow;

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
        /// The entity series instance UID (image_set types) or SOP instance UID (other types)
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// The entity modality
        /// </summary>
        [JsonPropertyName("modality")]
        public string Modality { get; set; }

        /// <summary>
        /// The entity description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The entity series instance UID
        /// </summary>
        [JsonPropertyName("series_uid")]
        public string SeriesUid { get; set; }

        /// <summary>
        /// The entity frame of reference UID
        /// </summary>
        [JsonPropertyName("frame_of_reference")]
        public string FrameOfReferenceUid { get; set; }

        /// <summary>
        /// The entity metadata (custom metrics)
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        /// <summary>
        /// The entity processing status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Entities within this entity
        /// </summary>
        [JsonPropertyName("entities")]
        public IList<EntitySummary> Entities { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Deletes the corresponding entity item asynchronously
        /// </summary>
        public async Task DeleteAsync()
        {
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/entities/{Id}");
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
            return $"{Type} | {Uid}";
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        /// <param name="patientId">The patient ID</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId, string patientId)
        {
            _proKnow = proKnow;
            WorkspaceId = workspaceId;
            PatientId = patientId;

            // Post-process deserialization of entities
            foreach (var entity in Entities)
            {
                entity.PostProcessDeserialization(_proKnow, WorkspaceId, PatientId);
            }
        }

        /// <summary>
        /// Gets the corresponding entity item asynchronously
        /// </summary>
        /// <returns>The corresponding entity item</returns>
        private async Task<EntityItem> GetEntityItemAsync()
        {
            if (!typeToRoutePartMap.ContainsKey(Type))
            {
                throw new ArgumentOutOfRangeException("The entity 'type' must be one of 'image_set', 'structure_set', 'plan', or 'dose'.");
            }
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/{typeToRoutePartMap[Type]}/{Id}");
            return DeserializeEntity(json);
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
            entityItem.PostProcessDeserialization(_proKnow, WorkspaceId);
            return entityItem;
        }
    }
}
