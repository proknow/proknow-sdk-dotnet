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
        private static readonly List<int> DEFAULT_RETRY_DELAYS = Enumerable.Repeat(200, 5).Concat(Enumerable.Repeat(1000, 29)).ToList();

        /// <inheritdoc/>
        public IList<int> RetryDelays { get; set; }

        private readonly ProKnowApi _proKnow;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { IgnoreNullValues = true };
        private static readonly IList<string> _terminalStatuses = new List<string>() { "completed", "pending", "failed" };

        /// <summary>
        /// Constructs an Uploads object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal Uploads(ProKnowApi proKnow)
        {
            _proKnow = proKnow;
            RetryDelays = DEFAULT_RETRY_DELAYS;
        }

        /// <inheritdoc/>
        public async Task<IList<UploadResult>> UploadAsync(WorkspaceItem workspaceItem, string path, UploadFileOverrides overrides = null)
        {
            // Get a list of all of the files to be uploaded
            var batchPaths = new List<string>();
            AddFiles(batchPaths, path);

            // Upload the files
            return await UploadFilesAsync(workspaceItem.Id, batchPaths, overrides);
        }

        /// <inheritdoc/>
        public async Task<IList<UploadResult>> UploadAsync(string workspace, string path, UploadFileOverrides overrides = null)
        {
            // Resolve the workspace
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);

            // Upload the files
            return await UploadAsync(workspaceItem, path, overrides);
        }

        /// <inheritdoc/>
        public async Task<IList<UploadResult>> UploadAsync(WorkspaceItem workspaceItem, IList<string> paths,
            UploadFileOverrides overrides = null)
        {
            // Get a list of all of the files to be uploaded
            var batchPaths = new List<string>();
            AddFiles(batchPaths, paths);

            // Upload the files
            return await UploadFilesAsync(workspaceItem.Id, batchPaths, overrides);
        }

        /// <inheritdoc/>
        public async Task<IList<UploadResult>> UploadAsync(string workspace, IList<string> paths, 
            UploadFileOverrides overrides = null)
        {
            // Resolve the workspace
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);

            // Upload the files
            return await UploadAsync(workspaceItem, paths, overrides);
        }

        /// <inheritdoc/>
        public async Task<IList<UploadProcessingResult>> GetUploadProcessingResultsAsync(WorkspaceItem workspace, IList<UploadResult> uploadResults)
        {
            // Create the collection of processing results for the provided set of uploads
            var idToUploadProcessingResult = new Dictionary<string, UploadProcessingResult>();

            // Create the collection of unresolved uploads
            var unresolvedUploadResults = uploadResults.ToList();
            var unresolvedUploadIds = uploadResults.Select(t => t.Id);

            // Loop until retries are exhausted
            var retryDelayIndex = 0;
            while (true)
            {
                // Loop until ProKnow has no more pages of upload processing results
                Dictionary<string, object> queryParameters = null;
                while (true)
                {
                    // Get the next page of upload processing results by filtering those updated after the last one to reach terminal status on the previous page
                    var responseJson = await _proKnow.Requestor.GetAsync($"/workspaces/{workspace.Id}/uploads/", null, queryParameters);
                    var thisPageUploadProcessingResults = JsonSerializer.Deserialize<IList<UploadProcessingResult>>(responseJson);

                    // Create collection to hold resolved upload IDs from this query
                    var resolvedUploadIds = new List<string>();

                    // Loop for each unresolved upload
                    foreach (var unresolvedUpload in unresolvedUploadResults)
                    {
                        // Search for corresponding upload in the query response
                        var uploadProcessingResult = thisPageUploadProcessingResults.FirstOrDefault(x => x.Id == unresolvedUpload.Id);

                        // If the upload was found
                        if (uploadProcessingResult != null)
                        {
                            // Overwrite the filename in case of duplicate content (ProKnow returns the original filename rather than the one just uploaded)
                            uploadProcessingResult.Path = unresolvedUpload.Path;

                            // Save the upload processing result
                            idToUploadProcessingResult[uploadProcessingResult.Id] = uploadProcessingResult;

                            // If its processing has reached a terminal status
                            if (_terminalStatuses.Contains(uploadProcessingResult.Status))
                            {
                                // Add the ID to the list of resolved upload IDs for this query
                                resolvedUploadIds.Add(uploadProcessingResult.Id);
                            }
                        }
                    }

                    // Remove any uploads that reached terminal status from the collection of unresolved uploads
                    foreach (var resolvedUploadId in resolvedUploadIds)
                    {
                        var index = unresolvedUploadResults.FindIndex(t => t.Id == resolvedUploadId);
                        unresolvedUploadResults.RemoveAt(index);
                    }

                    // If there are still unresolved uploads and possibly another page of results
                    if (unresolvedUploadResults.Count > 0 && thisPageUploadProcessingResults.Count > 0)
                    {
                        // Get the last upload on the current page whose processing has reached a terminal status
                        var lastTerminalUploadProcessingResult = thisPageUploadProcessingResults.LastOrDefault(t => _terminalStatuses.Contains(t.Status));

                        // If one was found
                        if (lastTerminalUploadProcessingResult != null)
                        {
                            // Update the query parameters to search for processed uploads updated after this one
                            if (queryParameters == null)
                            {
                                queryParameters = new Dictionary<string, object>();
                            }
                            queryParameters["updated"] = lastTerminalUploadProcessingResult.UpdatedAt;
                            queryParameters["after"] = lastTerminalUploadProcessingResult.Id;
                        }
                    }
                    else
                    {
                        break;
                    }
                } // End loop through pages of processing results

                // If there are still unresolved uploads and retries have not been exhausted
                if (unresolvedUploadResults.Count > 0 && retryDelayIndex < RetryDelays.Count)
                {
                    // Give the uploads some time to process
                    await Task.Delay(RetryDelays[retryDelayIndex++]);
                }
                else
                {
                    break;
                }
            } // End loop retries

            return idToUploadProcessingResult.Values.ToList();
        }

        /// <inheritdoc/>
        public async Task<IList<UploadProcessingResult>> GetUploadProcessingResultsAsync(string workspace, IList<UploadResult> uploadResults)
        {
            // Resolve the workspace
            var workspaceItem = await _proKnow.Workspaces.ResolveAsync(workspace);

            // Get the upload processing results
            return await GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
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
        /// <returns>The upload results</returns>
        private async Task<IList<UploadResult>> UploadFilesAsync(string workspaceId, IList<string> paths, UploadFileOverrides overrides)
        {
            var tasks = new List<Task>();
            var uploadResults = new ConcurrentBag<UploadResult>();
            foreach (var path in paths)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        var uploadResult = await UploadFileAsync(workspaceId, path, overrides);
                        uploadResults.Add(uploadResult);
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                }));
            }
            await Task.WhenAll(tasks);
            return uploadResults.ToList();
        }

        /// <summary>
        /// Uploads a file asychronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="path">The full path to the file to be uploaded</param>
        /// <param name="overrides">Optional overrides to be applied after the file is uploaded</param>
        /// <returns>The upload result</returns>
        private async Task<UploadResult> UploadFileAsync(string workspaceId, string path, UploadFileOverrides overrides = null)
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

            return new UploadResult(initiateFileUploadResponse.Id, initiateFileUploadResponse.Path, initiateFileUploadResponse.Status);
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
    }
}
