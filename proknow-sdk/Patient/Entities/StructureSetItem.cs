using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a structure set for a patient
    /// </summary>
    public class StructureSetItem : EntityItem
    {
        private const int RETRY_DELAY = 100;
        private const int MAX_TOTAL_RETRY_DELAY = 30000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

        //todo--_lock, _renewer

        /// <summary>
        /// Indicates whether this version is editable
        /// </summary>
        [JsonIgnore]
        public bool IsEditable { get; protected set; }

        /// <summary>
        /// Indicates whether this version is a draft
        /// </summary>
        [JsonIgnore]
        public bool IsDraft { get; protected set; }

        //todo--Rois

        //todo--Versions

        /// <summary>
        /// Type-specific entity data
        /// </summary>
        [JsonPropertyName("data")]
        public StructureSetData Data { get; set; }

        /// <summary>
        /// Downloads this entity asynchronously as a DICOM object to the specified folder
        /// </summary>
        /// <param name="folder">The full path to the destination folder</param>
        /// <returns>The full path to the file downloaded</returns>
        public override Task<string> DownloadAsync(string folder)
        {
            if (IsDraft)
            {
                throw new ApplicationException("Draft versions of structure sets cannot be downloaded.");
            }
            if (File.Exists(folder))
            {
                throw new ArgumentException($"The destination folder path '{folder}' is a path to an existing file.");
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var file = Path.Combine(folder, $"RS.{Uid}.dcm");
            return _requestor.StreamAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/versions/{Data.VersionId}/dicom", file);
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="requestor">Issues requests to the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal override void PostProcessDeserialization(Requestor requestor, string workspaceId)
        {
            base.PostProcessDeserialization(requestor, workspaceId);

            //todo--_lock = null; _renewer = null

            IsEditable = false;
            IsDraft = false;

            //todo--rois

            //todo--versions
        }

        private async void WaitForReadyStatus()
        {
            var numberOfRetries = 0;
            while (true)
            {
                var jsonString = await _requestor.GetAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/versions/{Data.VersionId}/status");
                var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
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
