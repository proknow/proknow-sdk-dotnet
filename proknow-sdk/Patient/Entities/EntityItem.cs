using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// The base class for entities in a ProKnow patient
    /// </summary>
    public abstract class EntityItem
    {
        /// <summary>
        /// Root object for interfacing with the ProKnow API
        /// </summary>
        protected ProKnow _proKnow;

        /// <summary>
        /// The patient workspace ID
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; internal set; }

        /// <summary>
        /// The patient ProKnow ID
        /// </summary>
        [JsonPropertyName("patient")]
        public string PatientId { get; set; }

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
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Interacts with scorecards for this entity
        /// </summary>
        [JsonIgnore]
        public EntityScorecards Scorecards { get; private set; }

        /// <summary>
        /// Deletes this entity asynchronously
        /// </summary>
        public async Task DeleteAsync()
        {
            await _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/entities/{Id}");
        }

        /// <summary>
        /// Downloads this entity asynchronously as DICOM object(s) to the specified folder
        /// </summary>
        /// <param name="root">The full path to the destination root folder</param>
        /// <returns>The full path to the destination folder (root or a sub-folder) to which the file(s) were downloaded</returns>
        public abstract Task<string> DownloadAsync(string root);

        /// <summary>
        /// Asynchronously resolves the metadata to a dictionary of custom metric names and values
        /// </summary>
        /// <returns>A dictionary of custom metric names and values</returns>
        public async Task<IDictionary<string, object>> GetMetadataAsync()
        {
            var customMetricItems = await Task.WhenAll(Metadata.Keys.Select(async (k) =>
                await _proKnow.CustomMetrics.ResolveAsync(k)));
            var metadata = new Dictionary<string, object>();
            foreach (var customMetricItem in customMetricItems)
            {
                metadata.Add(customMetricItem.Name, Metadata[customMetricItem.Id]);
            }
            return metadata;
        }

        /// <summary>
        /// Saves changes to an entity asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            var schema = new EntitySaveSchema
            {
                Description = Description,
                Metadata = Metadata
            };
            var content = new StringContent(JsonSerializer.Serialize(schema), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/workspaces/{WorkspaceId}/entities/{Id}", null, content);
        }

        /// <summary>
        /// Sets the metadata to an encoded version of the provided metadata
        /// </summary>
        /// <param name="metadata">A dictionary of custom metric names and values</param>
        public async Task SetMetadataAsync(IDictionary<string, object> metadata)
        {
            var resolvedMetadata = new Dictionary<string, object>();
            var tasks = new List<Task>();
            foreach (var key in metadata.Keys)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var customMetric = await _proKnow.CustomMetrics.ResolveByNameAsync(key);
                    resolvedMetadata.Add(customMetric.Id, metadata[key]);
                }));
            }
            await Task.WhenAll(tasks);
            Metadata = resolvedMetadata;
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
        internal virtual void PostProcessDeserialization(ProKnow proKnow, string workspaceId)
        {
            _proKnow = proKnow;
            WorkspaceId = workspaceId;

            // Convert metadata (custom metric) values from JsonElement to string, double, or int
            var metadata = new Dictionary<string, object>();
            foreach (var key in Metadata.Keys)
            {
                switch (((JsonElement)Metadata[key]).ValueKind)
                {
                    case JsonValueKind.String:
                        metadata.Add(key, ((JsonElement)Metadata[key]).GetString());
                        break;
                    case JsonValueKind.Number:
                        var numberAsDouble = ((JsonElement)Metadata[key]).GetDouble();
                        var numberAsInteger = (int)numberAsDouble;
                        if (numberAsDouble == numberAsInteger)
                        {
                            metadata.Add(key, numberAsInteger);
                        }
                        else
                        {
                            metadata.Add(key, numberAsDouble);
                        }
                        break;
                    default:
                        // leave value as is
                        metadata.Add(key, Metadata[key]);
                        break;
                }
            }
            Metadata = metadata;
            Scorecards = new EntityScorecards(_proKnow, WorkspaceId, Id);
        }
    }
}
