using ProKnow.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProKnow.Upload
{
    /// <summary>
    /// Interacts with uploads in ProKnow organization
    /// </summary>
    public class Uploads : IUploads
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(4);

        private const int RETRY_DELAY = 200;
        private const int MAX_TOTAL_RETRY_DELAY = 30000;
        private const int MAX_RETRIES = MAX_TOTAL_RETRY_DELAY / RETRY_DELAY;

        private readonly ProKnowApi _proKnow;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
        private readonly IList<string> _terminalStatuses = new List<string>() { "completed", "pending", "failed" };

        /// <summary>
        /// Constructs an Uploads object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal Uploads(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
        }

        /// <inheritdoc/>
        public async Task<UploadBatch> UploadAsync(WorkspaceItem workspaceItem, string path, UploadFileOverrides overrides = null,
            bool doWait = true)
        {
            // Get a list of all the files to be uploaded
            var batchPaths = new List<string>();
            AddFiles(batchPaths, path);

            // Upload the files and process the results
            return await UploadFilesAsync(workspaceItem.Id, batchPaths, overrides, doWait);
        }

        /// <inheritdoc/>
        public async Task<UploadBatch> UploadAsync(string workspace, string path, UploadFileOverrides overrides = null,
            bool doWait = true)
        {
            // Resolve the workspace ID
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);

            // Get a list of all the files to be uploaded
            var batchPaths = new List<string>();
            AddFiles(batchPaths, path);

            // Upload the files and process the results
            return await UploadFilesAsync(workspaceItem.Id, batchPaths, overrides, doWait);
        }

        /// <inheritdoc/>
        public async Task<UploadBatch> UploadAsync(WorkspaceItem workspaceItem, IList<string> paths,
            UploadFileOverrides overrides = null, bool doWait = true)
        {
            // Get a list of all the files to be uploaded
            var batchPaths = new List<string>();
            AddFiles(batchPaths, paths);

            // Upload the files and process the results
            return await UploadFilesAsync(workspaceItem.Id, batchPaths, overrides, doWait);
        }

        /// <inheritdoc/>
        public async Task<UploadBatch> UploadAsync(string workspace, IList<string> paths,
            UploadFileOverrides overrides = null, bool doWait = true)
        {
            // Resolve the workspace ID
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);

            // Get a list of all the files to be uploaded
            var batchPaths = new List<string>();
            AddFiles(batchPaths, paths);

            // Upload the files and process the results
            return await UploadFilesAsync(workspaceItem.Id, batchPaths, overrides, doWait);
        }

        /// <summary>
        /// Adds paths (recursively) to a batch
        /// </summary>
        /// <param name="batchPaths">The paths in the batch</param>
        /// <param name="paths">The paths to add to the batch</param>
        private void AddFiles(List<string> batchPaths, IList<string> paths)
        {
            foreach (var path in paths)
            {
                AddFiles(batchPaths, path);
            }
        }

        /// <summary>
        /// Add a path to a batch
        /// </summary>
        /// <param name="batchPaths">The paths in the batch</param>
        /// <param name="path">The path to add to the batch</param>
        private void AddFiles(List<string> batchPaths, string path)
        {
            var fileAttributes = File.GetAttributes(path);
            if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                batchPaths.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
            }
            else
            {
                batchPaths.Add(path);
            }
        }

        /// <summary>
        /// Uploads files asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="paths">The full paths to the files to be uploaded</param>
        /// <param name="overrides">Optional overrides to be applied after the file is uploaded</param>
        /// <param name="doWait">Indicates whether to wait until all uploads reach a terminal state</param>
        /// <returns>The result of the batch upload or null if not waiting for all uploads to reach a terminal state</returns>
        private async Task<UploadBatch> UploadFilesAsync(string workspaceId, IList<string> paths, UploadFileOverrides overrides, bool doWait)
        {
            var tasks = new List<Task>();
            var initiateFileUploadResponses = new ConcurrentBag<InitiateFileUploadResponse>();
            foreach (var path in paths)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        var uploadFileResults = await UploadFileAsync(workspaceId, path, overrides);
                        initiateFileUploadResponses.Add(uploadFileResults);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }));
            }
            await Task.WhenAll(tasks);
            if (doWait)
            {
                return await ProcessUploadResults(workspaceId, initiateFileUploadResponses.ToArray());
            }
            return null;
        }

        /// <summary>
        /// Uploads a file asychronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="path">The full path to the file to be uploaded</param>
        /// <param name="overrides">Optional overrides to be applied after the file is uploaded</param>
        /// <returns>The response to the file upload request</returns>
        private async Task<InitiateFileUploadResponse> UploadFileAsync(string workspaceId, string path, UploadFileOverrides overrides = null)
        {
            // Initiate the file upload
            var initiateFileUploadResponse = await InitiateFileUploadAsync(workspaceId, path, overrides);

            // If the file has not already been uploaded
            if (initiateFileUploadResponse.Status == "uploading")
            {
                // Upload the file content
                await UploadChunkAsync(initiateFileUploadResponse);
            }
            else
            {
                // Overwrite the filename in case of duplicate content (ProKnow returns the original filename rather than the one just uploaded)
                initiateFileUploadResponse.Path = path;
            }

            return initiateFileUploadResponse;
        }

        /// <summary>
        /// Initiates a file upload asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the destination workspace</param>
        /// <param name="path">The full path to the file</param>
        /// <param name="overrides">Optional overrides to be applied to the data</param>
        /// <returns>The response from the file upload initiation</returns>
        private async Task<InitiateFileUploadResponse> InitiateFileUploadAsync(string workspaceId, string path, UploadFileOverrides overrides = null)
        {
            var requestBody = BuildInitiateFileUploadRequestBody(path, overrides);
            var json = await _proKnow.Requestor.PostAsync($"/workspaces/{workspaceId}/uploads/", null, requestBody);
            return JsonSerializer.Deserialize<InitiateFileUploadResponse>(json);
        }

        /// <summary>
        /// Builds the body for an initiate file upload request
        /// </summary>
        /// <param name="path">The full path to the file</param>
        /// <param name="overrides">Optional overrides to be applied to the data</param>
        /// <returns>The content for the body</returns>
        private HttpContent BuildInitiateFileUploadRequestBody(string path, UploadFileOverrides overrides = null)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var uploadFileRequest = new InitiateFileUploadRequestBody
                    {
                        Checksum = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower(),
                        Path = path,
                        Filesize = stream.Length,
                        IsMultipart = false,
                        Overrides = overrides
                    };
                    var json = JsonSerializer.Serialize(uploadFileRequest, _jsonSerializerOptions);
                    return new StringContent(json, Encoding.UTF8, "application/json");
                }
            }
        }

        /// <summary>
        /// Uploads file contents as a single chunk
        /// </summary>
        /// <param name="initiateFileUploadResponse">The response from the request to initiate the file upload</param>
        private async Task UploadChunkAsync(InitiateFileUploadResponse initiateFileUploadResponse)
        {
            var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("Proknow-Key", initiateFileUploadResponse.Key) };
            var filesize = initiateFileUploadResponse.Filesize.ToString();
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent("1"), "flowChunkNumber");
                content.Add(new StringContent(filesize), "flowChunkSize");
                content.Add(new StringContent(filesize), "flowCurrentChunkSize");
                content.Add(new StringContent("1"), "flowTotalChunks");
                content.Add(new StringContent(filesize), "flowTotalSize");
                content.Add(new StringContent(initiateFileUploadResponse.Identifier), "flowIdentifier");
                content.Add(new StringContent(initiateFileUploadResponse.Path), "flowFilename");
                content.Add(new StringContent(initiateFileUploadResponse.IsMultipart.ToString()), "flowMultipart");
                using (var fs = File.OpenRead(initiateFileUploadResponse.Path))
                {
                    content.Add(new StreamContent(fs), "file", initiateFileUploadResponse.Path);
                    await _proKnow.Requestor.PostAsync("/uploads/chunks", headerKeyValuePairs, content);
                }
            }
        }

        /// <summary>
        /// Waits for a batch upload to complete and processes the results
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="initiateUploadFileResponses">The results for each file upload</param>
        /// <returns>The batch results</returns>
        private async Task<UploadBatch> ProcessUploadResults(string workspaceId,
            InitiateFileUploadResponse[] initiateUploadFileResponses)
        {
            // Create the collection of status results that will be used to create returned upload batch
            var batchUploadStatusResults = new List<UploadStatusResult>();

            // Create the collection of unresolved uploads
            var unresolvedUploads = initiateUploadFileResponses.ToList();

            Dictionary<string, object> queryParameters = null;
            var numberOfRetries = 0;
            while (unresolvedUploads.Count > 0 && numberOfRetries < MAX_RETRIES)
            {
                // Query the current status of the uploads, filtering those whose status has changed since the previous query
                var uploadStatusResultsJson = await _proKnow.Requestor.GetAsync($"/workspaces/{workspaceId}/uploads/", null, queryParameters);
                var uploadStatusResults = JsonSerializer.Deserialize<IList<UploadStatusResult>>(uploadStatusResultsJson);

                // Create collection to hold resolved upload IDs
                var resolvedUploadIds = new List<string>();

                // Loop for each unresolved upload
                foreach (var unresolvedUpload in unresolvedUploads)
                {
                    // Find corresponding upload status result
                    var uploadStatusResult = uploadStatusResults.FirstOrDefault(x => x.Id == unresolvedUpload.Id);

                    // If this upload has reached terminal status
                    if (uploadStatusResult != null && _terminalStatuses.Contains(uploadStatusResult.Status))
                    {
                        // Overwrite the filename in case of duplicate content (ProKnow returns the original filename rather than the one just uploaded)
                        uploadStatusResult.Path = unresolvedUpload.Path;

                        // Save the upload result
                        batchUploadStatusResults.Add(uploadStatusResult);

                        // Add the ID to the list of resolved upload IDs
                        resolvedUploadIds.Add(uploadStatusResult.Id);
                    }
                }

                // Remove any uploads that reached terminal status from the collection of unresolved uploads
                foreach (var resolvedUploadId in resolvedUploadIds)
                {
                    var index = unresolvedUploads.FindIndex(t => t.Id == resolvedUploadId);
                    unresolvedUploads.RemoveAt(index);
                }

                // If there are still unresolved uploads
                if (unresolvedUploads.Count > 0)
                {
                    // Get the last upload result to reach terminal status
                    var lastTerminalUploadResult = uploadStatusResults.LastOrDefault(t => _terminalStatuses.Contains(t.Status));

                    // If one was found
                    if (lastTerminalUploadResult != null)
                    {
                        // Update the query parameters to search for upload results after this one
                        if (queryParameters == null)
                        {
                            queryParameters = new Dictionary<string, object>();
                        }
                        queryParameters["updated"] = lastTerminalUploadResult.UpdatedAt;
                        queryParameters["after"] = lastTerminalUploadResult.Id;
                    }

                    // Give the updates some time to process
                    await Task.Delay(RETRY_DELAY);
                    numberOfRetries++;
                }
            }

            // Verify there are no unresolved uploads
            if (unresolvedUploads.Count > 0)
            {
                throw new ProKnowException($"Unable to resolve uploads for ${unresolvedUploads.Count} DICOM object.  Timed out after ${MAX_RETRIES} retries over ${MAX_TOTAL_RETRY_DELAY / 1000} sec.");
            }

            return new UploadBatch(_proKnow, workspaceId, batchUploadStatusResults);
        }
    }
}
