﻿using System;
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

        //todo--Add Versions property

        /// <summary>
        /// Type-specific entity data
        /// </summary>
        [JsonPropertyName("data")]
        public StructureSetData Data { get; set; }

        //todo--Implement ApproveAsync method

        //todo--Implement CreateRoiAsync method

        //todo--Implement DiscardAsync method

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
            var route = $"/workspaces/{WorkspaceId}/structuresets/{Id}/versions/{Data.VersionId}/dicom";
            return _proKnow.Requestor.StreamAsync(route, file);
        }

        //todo--Implement DraftAsync method

        //todo--Implement ReleaseLockAsync method

        //todo--Implement StartRenewer method

        //todo--Implement StopRenewer method

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal override void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId)
        {
            base.PostProcessDeserialization(proKnow, workspaceId);

            //todo--_lock = null; _renewer = null

            IsEditable = false;
            IsDraft = false;

            //todo--post process Versions deserialization
        }

        /// <summary>
        /// Waits until the structure set version status becomes "ready"
        /// </summary>
        private async void WaitForReadyStatus()
        {
            var route = $"/workspaces/{WorkspaceId}/structuresets/{Id}/versions/{Data.VersionId}/status";
            var numberOfRetries = 0;
            while (true)
            {
                var jsonString = await _proKnow.Requestor.GetAsync(route);
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
