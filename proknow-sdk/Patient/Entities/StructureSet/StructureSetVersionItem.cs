using System;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Patient.Entities.StructureSet
{
    /// <summary>
    /// Represents a version of a structure set
    /// </summary>
    /// <remarks>
    /// Objects of this class are instantiated by the <see cref="StructureSetVersions.QueryAsync()"/>
    /// </remarks>
    public class StructureSetVersionItem
    {
        private const int RETRY_DELAY = 100;
        private const int MAX_TOTAL_RETRY_DELAY = 30000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

        private ProKnowApi _proKnow;
        private StructureSetVersions _structureSetVersions;

        /// <summary>
        /// The ProKnow ID of the workspace
        /// </summary>
        [JsonIgnore]
        public string WorkspaceId { get; protected set; }

        /// <summary>
        /// The ProKnow ID of the structure set item
        /// </summary>
        [JsonIgnore]
        public string StructureSetId { get; protected set; }

        /// <summary>
        /// The ProKnow ID of this structure set version
        /// </summary>
        [JsonPropertyName("version")]
        public string VersionId { get; set; }

        /// <summary>
        /// The status of this version
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Indicates whether this is a structure set draft
        /// </summary>
        [JsonIgnore]
        public bool IsDraft { get; private set; }

        /// <summary>
        /// The label of this version
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>
        /// The message for this version
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Properties encountered during deserialization without matching members
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }

        /// <summary>
        /// Deletes this structure set version asynchronously
        /// </summary>
        /// <returns></returns>
        public Task DeleteAsync()
        {
            if (IsDraft)
            {
                throw new InvalidOperationError("Draft versions of structure sets cannot be deleted.");
            }
            return _structureSetVersions.DeleteAsync(VersionId);
        }

        /// <summary>
        /// Downloads this structure set asynchronously as a DICOM object to the specified folder or file
        /// </summary>
        /// <param name="path">The full path to the destination folder or file</param>
        /// <returns>The full path to the file to which the structure set was downloaded</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// If the provided path is an existing folder, the structure set will be saved to a file named
        /// RS.{SOP instance UID}.dcm.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the provided path is not an existing folder, the structure set will be saved to the provided path.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public async Task<string> DownloadAsync(string path)
        {
            if (IsDraft)
            {
                throw new InvalidOperationError("Draft versions of structure sets cannot be downloaded.");
            }
            string file;
            if (Directory.Exists(path))
            {
                file = Path.Combine(path, $"RS.{VersionId}.dcm");
            }
            else
            {
                var parentDirectoryInfo = Directory.GetParent(path);
                if (!parentDirectoryInfo.Exists)
                {
                    parentDirectoryInfo.Create();
                }
                file = path;
            }
            await WaitForReadyStatusAsync();
            var route = $"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}/versions/{VersionId}/dicom";
            return await _proKnow.Requestor.StreamAsync(route, file);
        }

        /// <summary>
        /// Gets the structure set item corresponding to this version asynchronously
        /// </summary>
        /// <returns>The structure set item corresponding to this version</returns>
        public Task<StructureSetItem> GetAsync()
        {
            if (IsDraft)
            {
                return _structureSetVersions.GetAsync("draft");
            }
            else
            {
                return _structureSetVersions.GetAsync(VersionId);
            }
        }

        /// <summary>
        /// Makes this the approved version of the structure set
        /// </summary>
        /// <returns>A structure set item corresponding to this version</returns>
        public async Task<StructureSetItem> RevertAsync()
        {
            if (IsDraft)
            {
                throw new InvalidOperationError("Structure sets cannot be reverted to draft versions.");
            }
            await _proKnow.Requestor.PostAsync($"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}/approve/{VersionId}");
            return await _structureSetVersions.GetAsync("approved");
        }

        /// <summary>
        /// Saves the label and message for this structure set version asynchronously
        /// </summary>
        public async Task SaveAsync()
        {
            if (IsDraft)
            {
                throw new ApplicationException("Draft versions of structure sets cannot be saved.");
            }
            var properties = new Dictionary<string, object>
            {
                { "label", Label },
                { "message", Message }
            };
            var requestJson = JsonSerializer.Serialize(properties);
            var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PutAsync($"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}/versions/{VersionId}", null, requestContent);
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return Label;
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">The root object for interfacing with the ProKnow API</param>
        /// <param name="structureSetVersions">The object for interfacing with structure set versions</param>
        internal void PostProcessDeserialization(ProKnowApi proKnow, StructureSetVersions structureSetVersions)
        {
            _proKnow = proKnow;
            _structureSetVersions = structureSetVersions;
            WorkspaceId = structureSetVersions.WorkspaceId;
            StructureSetId = structureSetVersions.StructureSetId;
            IsDraft = (Status == "draft");
        }

        /// <summary>
        /// Waits until the structure set version status becomes "ready"
        /// </summary>
        private async Task WaitForReadyStatusAsync()
        {
            var route = $"/workspaces/{WorkspaceId}/structuresets/{StructureSetId}/versions/{VersionId}/status";
            var numberOfRetries = 0;
            while (true)
            {
                var json = await _proKnow.Requestor.GetAsync(route);
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (keyValuePairs["status"] == "ready")
                {
                    break;
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
                        throw new TimeoutException($"Timeout while waiting for structure set version to reach ready status.");
                    }
                }
            }
        }
    }
}
