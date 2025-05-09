using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using static ProKnow.RtvRequestor;


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
            return await DeserializeEntity(json);
        }

        /// <summary>
        /// Creates an entity item from its JSON representation
        /// </summary>
        /// <param name="json">JSON representation of the entity item</param>
        /// <returns>An entity item</returns>
        private async Task<EntityItem> DeserializeEntity(string json)
        {

            // Converts Dicom to IEC Patient Coordinate System
            EntityItem entityItem;
            if (Type == "image_set")
            {
                var imageSetItem = JsonSerializer.Deserialize<ImageSetItem>(json);
                JsonElement body = await processDicom($"/imageset", imageSetItem.Data.Dicom, imageSetItem.Data.DicomToken, Type);
                var data = body.GetProperty("data");
                imageSetItem.Data.MinX = data.GetProperty("min_x").GetDouble();
                imageSetItem.Data.MaxX = data.GetProperty("max_x").GetDouble();
                imageSetItem.Data.MinY = data.GetProperty("min_z").GetDouble();
                imageSetItem.Data.MaxY = data.GetProperty("max_z").GetDouble();
                imageSetItem.Data.MinZ = data.GetProperty("max_y").GetDouble() * -1;
                imageSetItem.Data.MaxZ = data.GetProperty("min_y").GetDouble() * -1;
                imageSetItem.Data.MaxValue = data.GetProperty("max_value").GetDouble();
                imageSetItem.Data.MinValue = data.GetProperty("min_value").GetDouble();
                imageSetItem.Data.PatientPosition = data.GetProperty("patient_position").GetString();
                imageSetItem.Data.PhotometricInterpretation = data.GetProperty("photometric_interpretation").GetString();
                imageSetItem.Data.NumberOfColumns = data.GetProperty("resolution_u").GetUInt16();
                imageSetItem.Data.NumberOfRows = data.GetProperty("resolution_v").GetUInt16();
                imageSetItem.Data.NumberOfImages = (ushort)data.GetProperty("resolution_w").GetDouble();
                imageSetItem.Data.ColumnExtents = data.GetProperty("size_u").GetDouble();
                imageSetItem.Data.RowExtents = data.GetProperty("size_v").GetDouble();
                imageSetItem.Data.SliceExtents = data.GetProperty("size_w").GetDouble();
                imageSetItem.Data.ColumnSpacing = data.GetProperty("spacing_u").GetDouble();
                imageSetItem.Data.RowSpacing = data.GetProperty("spacing_v").GetDouble();
                imageSetItem.Data.SliceSpacing = data.GetProperty("spacing_w").GetDouble();
                imageSetItem.Data.HasUniformSliceSpacing = data.GetProperty("uniform_w").GetBoolean();
                imageSetItem.Data.RowXDirectionCosine = data.GetProperty("u_x").GetDouble();
                imageSetItem.Data.RowYDirectionCosine = data.GetProperty("u_z").GetDouble();
                imageSetItem.Data.RowZDirectionCosine = data.GetProperty("u_y").GetDouble() * -1;
                imageSetItem.Data.ColumnXDirectionCosine = data.GetProperty("v_x").GetDouble();
                imageSetItem.Data.ColumnYDirectionCosine = data.GetProperty("v_z").GetDouble();
                imageSetItem.Data.ColumnZDirectionCosine = data.GetProperty("v_y").GetDouble() * -1;
                imageSetItem.Data.ProcessedId = data.GetProperty("processed_id").GetString();

                // Iterate through images and create a dictionary
                var ImagesUidMap = data.GetProperty("images").EnumerateArray().ToDictionary(
                    imageElement => imageElement.GetProperty("uid").GetString());

                // Update images information
                foreach (var image in imageSetItem.Data.Images)
                {
                    if (ImagesUidMap.ContainsKey(image.Uid))
                    {
                        JsonElement matchingElement = ImagesUidMap[image.Uid];
                        image.RescaleIntercept = matchingElement.GetProperty("b").GetDouble();
                        image.RescaleSlope = matchingElement.GetProperty("m").GetDouble();
                        image.PositionX = matchingElement.GetProperty("pos_x").GetDouble();
                        image.PositionY = matchingElement.GetProperty("pos_z").GetDouble();
                        image.PositionZ = matchingElement.GetProperty("pos_y").GetDouble() * -1;
                        image.Tag = matchingElement.GetProperty("processed_id").GetString();
                    }
                }
                imageSetItem.Data.Images = imageSetItem.Data.Images.OrderBy(i => i.Position).ToList();
                entityItem = imageSetItem;
            }
            else if (Type == "structure_set")
            {
                entityItem = JsonSerializer.Deserialize<StructureSetItem>(json);
            }
            else if (Type == "plan")
            {
                entityItem = JsonSerializer.Deserialize<PlanItem>(json);
            }
            else if (Type == "dose")
            {
                var doseItem = JsonSerializer.Deserialize<DoseItem>(json);
                JsonElement body = await processDicom($"/dose", doseItem.Data.Dicom, doseItem.Data.DicomToken, Type);
                var data = body.GetProperty("data");
                doseItem.Data.MinX = data.GetProperty("min_x").GetDouble();
                doseItem.Data.MaxX = data.GetProperty("max_x").GetDouble();
                doseItem.Data.MinY = data.GetProperty("min_z").GetDouble();
                doseItem.Data.MaxY = data.GetProperty("max_z").GetDouble();
                doseItem.Data.MinZ = data.GetProperty("max_y").GetDouble() * -1;
                doseItem.Data.MaxZ = data.GetProperty("min_y").GetDouble() * -1;
                doseItem.Data.MaxValue = data.GetProperty("max_value").GetDouble();
                doseItem.Data.MinValue = data.GetProperty("min_value").GetDouble();
                doseItem.Data.ResolutionX = data.GetProperty("resolution_x").GetInt32();
                doseItem.Data.ResolutionY = data.GetProperty("resolution_z").GetInt32();
                doseItem.Data.ResolutionZ = data.GetProperty("resolution_y").GetInt32();
                doseItem.Data.SpacingX = data.GetProperty("spacing_x").GetDouble();
                doseItem.Data.SpacingY = data.GetProperty("spacing_z").GetDouble();
                doseItem.Data.SpacingZ = data.GetProperty("spacing_y").GetDouble();
                doseItem.Data.SizeX = data.GetProperty("size_x").GetDouble();
                doseItem.Data.SizeY = data.GetProperty("size_z").GetDouble();
                doseItem.Data.SizeZ = data.GetProperty("size_y").GetDouble();
                doseItem.Data.PixelIntercept = data.GetProperty("pixel_intercept").GetDouble();
                doseItem.Data.PixelSlope = data.GetProperty("pixel_slope").GetDouble();
                doseItem.Data.SummationType = data.GetProperty("summation_type").GetString();
                doseItem.Data.DoseType = data.GetProperty("type").GetString();
                doseItem.Data.DoseUnits = data.GetProperty("units").GetString();
                doseItem.Data.ProcessedId = data.GetProperty("processed_id").GetString();
                doseItem.Data.Slices = data.GetProperty("slices").EnumerateArray()
                    .Select(sliceElement => new DoseSlice
                    {
                        Tag = sliceElement.GetProperty("id").GetString(),
                        Position = sliceElement.GetProperty("pos").GetDouble()
                    }).ToList();

                entityItem = doseItem;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "The entity 'type' must be one of 'image_set', 'structure_set', 'plan', or 'dose'.");
            }
            entityItem.PostProcessDeserialization(_proKnow, WorkspaceId);
            return entityItem;
        }

        private async Task<JsonElement> processDicom(string route, string[] dicom, string dicomToken, string type)
        {

            // Post request to RTV until the status is completed
            var properties = new Dictionary<string, object>() { { "data", dicom } };
            var content = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            ObjectType objectType;
            switch (type)
            {
                case "image_set":
                    objectType = ObjectType.ImageSet;
                    break;
                case "structure_set":
                    objectType = ObjectType.StructureSet;
                    break;
                case "plan":
                    objectType = ObjectType.Plan;
                    break;
                case "dose":
                    objectType = ObjectType.Dose;
                    break;
                default:
                    throw new ArgumentException($"Argument 'type' must be one of 'image_set', 'structure_set', 'plan', or 'dose'.");
            }
            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization", "Bearer " + dicomToken),
                new KeyValuePair<string, string>("Accept-Version", await _proKnow.RtvRequestor.GetApiVersion(objectType))
            };
            int numberOfRetries = 0;
            JsonElement postResponseJson;
            while (true)
            {
                var postResponse = await _proKnow.RtvRequestor.PostAsync(route, headers, content);
                postResponseJson = JsonSerializer.Deserialize<JsonElement>(postResponse);
                if (postResponseJson.GetProperty("status").GetString() == "completed")
                {
                    return postResponseJson;
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
    }
}
