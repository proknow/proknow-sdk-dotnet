using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a structure set for a patient
    /// </summary>
    public class StructureSetItem : EntityItem, IAsyncDisposable
    {
        private const int RETRY_DELAY = 100;
        private const int MAX_TOTAL_RETRY_DELAY = 30000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

        private StructureSetDraftLockRenewer _draftLockRenewer;
        private bool _isDisposed;

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

        /// <summary>
        /// The draft lock
        /// </summary>
        [JsonIgnore]
        public StructureSetDraftLock DraftLock { get; set; }

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
        public override Task<string> DownloadAsync(string path)
        {
            if (IsDraft)
            {
                throw new ApplicationException("Draft versions of structure sets cannot be downloaded.");
            }
            string file = null;
            if (Directory.Exists(path))
            {
                file = Path.Combine(path, $"RS.{Uid}.dcm");
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
            var route = $"/workspaces/{WorkspaceId}/structuresets/{Id}/versions/{Data.VersionId}/dicom";
            return _proKnow.Requestor.StreamAsync(route, file);
        }

        /// <summary>
        /// Creates a draft if one does not already exist for the structure set or obtains the lock
        /// </summary>
        /// <returns>A draft structure set item</returns>
        public async Task<StructureSetItem> DraftAsync()
        {
            StructureSetDraftLock draftLock = null;
            try
            {
                var lockJson = await _proKnow.Requestor.PostAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft");
                draftLock = JsonSerializer.Deserialize<StructureSetDraftLock>(lockJson);
            }
            catch (ProKnowHttpException ex)
            {
                if (ex.StatusCode != "Conflict")
                {
                    throw ex;
                }
                var lockJson = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft/lock");
                draftLock = JsonSerializer.Deserialize<StructureSetDraftLock>(lockJson);
            }
            var queryParameters = new Dictionary<string, object>();
            queryParameters.Add("version", "draft");
            var responseJson = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}", queryParameters);
            var structureSetItem = JsonSerializer.Deserialize<StructureSetItem>(responseJson);
            structureSetItem.PostProcessDeserialization(_proKnow, WorkspaceId);
            structureSetItem.IsEditable = true;
            structureSetItem.IsDraft = true;
            structureSetItem.DraftLock = draftLock;
            structureSetItem._draftLockRenewer = new StructureSetDraftLockRenewer(_proKnow, structureSetItem);
            structureSetItem._draftLockRenewer.Start();
            structureSetItem._isDisposed = false;
            return structureSetItem;
        }

        /// <summary>
        /// Releases the draft lock asynchronously
        /// </summary>
        public async Task ReleaseLockAsync()
        {
            if (IsEditable)
            {
                await _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft/lock/{DraftLock.Id}");
                DraftLock = null;
                IsEditable = false;
            }
        }

        /// <summary>
        /// Starts the draft lock renewer
        /// </summary>
        public void StartRenewer()
        {
            if (IsEditable)
            {
                _draftLockRenewer = new StructureSetDraftLockRenewer(_proKnow, this);
                _draftLockRenewer.Start();
            }
        }

        /// <summary>
        /// Stops the draft lock renewer
        /// </summary>
        public void StopRenewer()
        {
            if (IsEditable && _draftLockRenewer != null)
            {
                _draftLockRenewer.Stop();
                _draftLockRenewer = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await DisposeAsyncCore();

            // Dispose of managed resources.
            Dispose(false);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            StopRenewer();
            await ReleaseLockAsync();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects).
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.

            _isDisposed = true;
        }



        ///// <summary>
        ///// Disposes of managed resources
        ///// </summary>
        //public async ValueTask Dispose()
        //{
        //    await Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        ///// <summary>
        ///// Disposes of managed resources
        ///// </summary>
        ///// <param name="disposing">True if disposing</param>
        //protected virtual async ValueTask Dispose(bool disposing)
        //{
        //    if (!_isDisposed)
        //        return;

        //    if (disposing)
        //    {
        //        StopRenewer();
        //        await ReleaseLockAsync();
        //    }

        //    _isDisposed = true;
        //}

        ///// <summary>
        ///// Disposes of managed resources asynchronously
        ///// </summary>
        //public virtual ValueTask DisposeAsync()
        //{
        //    try
        //    {
        //        //todo--fix this!!!
        //        Dispose(true);
        //        GC.SuppressFinalize(this);
        //        return default;
        //    }
        //    catch (Exception exception)
        //    {
        //        return new ValueTask(Task.FromException(exception));
        //    }
        //}

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal override void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId)
        {
            base.PostProcessDeserialization(proKnow, workspaceId);

            _draftLockRenewer = null;
            _isDisposed = false;
            IsEditable = false;
            IsDraft = false;
            DraftLock = null;

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
