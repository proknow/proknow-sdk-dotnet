using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProKnow.Exceptions;
using ProKnow.Patient.Entities.StructureSet;

namespace ProKnow.Patient.Entities
{
    /// <summary>
    /// Represents a structure set for a patient
    /// </summary>
    public class StructureSetItem : EntityItem, IDisposable
    {
        private StructureSetDraftLockRenewer _draftLockRenewer;
        private JsonSerializerOptions _jsonSerializerOptions;
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
        public bool IsDraft { get; internal set; }

        /// <summary>
        /// The draft lock
        /// </summary>
        [JsonIgnore]
        public StructureSetDraftLock DraftLock { get; set; }

        /// <summary>
        /// The regions of interest (ROIs)
        /// </summary>
        [JsonIgnore]
        public StructureSetRoiItem[] Rois { get; internal set; }

        /// <summary>
        /// The object for interacting with versions of the structure set
        /// </summary>
        [JsonIgnore]
        public StructureSetVersions Versions { get; protected set; }

        /// <summary>
        /// Type-specific entity data
        /// </summary>
        [JsonPropertyName("data")]
        public StructureSetData Data { get; set; }

        /// <summary>
        /// Approves a structure set draft asynchronously
        /// </summary>
        /// <param name="label">The label</param>
        /// <param name="message">The message</param>
        /// <returns>The newly approved structure set</returns>
        public async Task<StructureSetItem> ApproveAsync(string label = null, string message = null)
        {
            if (!IsEditable)
            {
                throw new InvalidOperationError("Item is not editable");
            }
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Lock", DraftLock.Id) };
            var rois = new List<Dictionary<string, object>>();
            foreach (var roi in Rois)
            {
                rois.Add(new Dictionary<string, object>() { { "id", roi.Id }, { "tag", roi.Tag } });
            }
            var properties = new Dictionary<string, object>() { { "version", Data.VersionId }, { "rois", rois }, { "label", label }, { "message", message } };
            var requestContent = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PostAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft/approve", headerKeyValuePairs, requestContent);
            StopRenewer();
            IsEditable = false;
            IsDraft = false;
            DraftLock = null;
            return await Versions.GetAsync("approved");
        }

        /// <summary>
        /// Creates a new ROI as part of the draft structure set
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="color">The RGB colors</param>
        /// <param name="type">The type</param>
        /// <remarks>
        /// The valid types are'EXTERNAL', 'PTV', 'CTV', 'GTV', 'TREATED_VOLUME', 'IRRAD_VOLUME', 'BOLUS', 'AVOIDANCE',
        /// 'ORGAN', 'MARKER', 'REGISTRATION', 'ISOCENTER', 'CONTRAST_AGENT', 'CAVITY', 'BRACHY_CHANNEL',
        /// 'BRACHY_ACCESSORY', 'BRACHY_SRC_APP', 'BRACHY_CHNL_SHLD', 'SUPPORT', 'FIXATION', 'DOSE_REGION', 'CONTROL'
        /// </remarks>
        /// <returns>The created ROI</returns>
        public async Task<StructureSetRoiItem> CreateRoiAsync(string name, Color color, string type)
        {
            if (!IsEditable)
            {
                throw new InvalidOperationError("Item is not editable");
            }
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Lock", DraftLock.Id) };
            var properties = new Dictionary<string, object>() { { "name", name }, { "color", color }, { "type", type } };
            var requestContent = new StringContent(JsonSerializer.Serialize(properties, _jsonSerializerOptions),
                Encoding.UTF8, "application/json");
            var responseJson = await _proKnow.Requestor.PostAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft/rois",
                headerKeyValuePairs, requestContent);
            var structureSetRoiItem = JsonSerializer.Deserialize<StructureSetRoiItem>(responseJson);
            Rois = Rois.Concat(new StructureSetRoiItem[1] { structureSetRoiItem }).ToArray();
            structureSetRoiItem.PostProcessDeserialization(_proKnow, WorkspaceId, this);
            return structureSetRoiItem;
        }

        /// <summary>
        /// Discards a structure set draft
        /// </summary>
        public async Task DiscardAsync()
        {
            if (!IsEditable)
            {
                throw new InvalidOperationError("Item is not editable");
            }
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("ProKnow-Lock", DraftLock.Id) };
            var rois = new List<Dictionary<string, object>>();
            foreach (var roi in Rois)
            {
                rois.Add(new Dictionary<string, object>() { { "id", roi.Id }, { "tag", roi.Tag } });
            }
            var properties = new Dictionary<string, object>() { { "version", Data.VersionId }, { "rois", rois } };
            var requestContent = new StringContent(JsonSerializer.Serialize(properties), Encoding.UTF8, "application/json");
            await _proKnow.Requestor.PostAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft/discard", headerKeyValuePairs, requestContent);
            StopRenewer();
            IsEditable = false;
            DraftLock = null;
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
                // Create a structure set draft
                var lockJson = await _proKnow.Requestor.PostAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft");
                draftLock = JsonSerializer.Deserialize<StructureSetDraftLock>(lockJson);
            }
            catch (ProKnowHttpException ex)
            {
                if (ex.StatusCode != "Conflict")
                {
                    throw ex;
                }

                // Get the structure set draft lock
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
        /// Refreshes this structure set item asynchronously
        /// </summary>
        public async Task RefreshAsync()
        {
            if (IsDraft)
            {
                throw new InvalidOperationError("Cannot refresh a draft structure set");
            }
            var json = await _proKnow.Requestor.GetAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}");
            var structureSetItem = JsonSerializer.Deserialize<StructureSetItem>(json);
            Data = structureSetItem.Data;
            Rois = Data.Rois;
            foreach (var roi in Rois)
            {
                roi.PostProcessDeserialization(_proKnow, WorkspaceId, this);
            }
        }

        /// <summary>
        /// Releases the draft lock
        /// </summary>
        public void ReleaseLock()
        {
            if (IsEditable)
            {
                try
                {
                    _proKnow.Requestor.DeleteAsync($"/workspaces/{WorkspaceId}/structuresets/{Id}/draft/lock/{DraftLock.Id}").Wait();
                }
                catch (AggregateException ae)
                {
                    foreach (var ex in ae.InnerExceptions)
                    {
                        // Handle the situation where the lock expired before we could delete it (e.g., when unit testing)
                        if (!(ex is ProKnowHttpException) ||
                            ex.Message != "HttpError(Forbidden, Structure set is not currently locked for editing)")
                        {
                            throw ex;
                        }
                    }
                }
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
            if (_draftLockRenewer != null)
            {
                _draftLockRenewer.Stop();
                _draftLockRenewer = null;
            }
        }

        /// <summary>
        /// Finishes initialization of object after deserialization from JSON
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        /// <param name="workspaceId">The workspace ID</param>
        internal override void PostProcessDeserialization(ProKnowApi proKnow, string workspaceId)
        {
            base.PostProcessDeserialization(proKnow, workspaceId);

            _draftLockRenewer = null;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new ColorJsonConverter());
            _isDisposed = false;
            IsEditable = false;
            IsDraft = false;
            DraftLock = null;
            Rois = Data.Rois;
            foreach (var roi in Rois)
            {
                roi.PostProcessDeserialization(proKnow, workspaceId, this);
            }
            Versions = new StructureSetVersions(_proKnow, WorkspaceId, Id);
        }

        #region IDisposable Implementation

        /// <summary>
        /// Dispose of resources
        /// </summary>
        /// <param name="disposing">If true, this method has been called directly or indirectly by a user's code and
        /// managed and unmanaged resources can be disposed. If false, the method has been called by the runtime from
        /// inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    StopRenewer();
                    ReleaseLock();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer (nothing to do)
                // Set large fields to null (nothing to do)
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Dispose of resources
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
