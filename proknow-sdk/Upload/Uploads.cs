using Microsoft.Extensions.Logging;
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
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ProKnow.Upload
{
    /// <summary>
    /// Interacts with uploads in ProKnow organization
    /// </summary>
    public class Uploads : IUploads
    {
        // Note that ProKnow requires a minimum chunk size of 5 MB
        private static readonly int UPLOAD_CHUNK_SIZE_IN_BYTES = 5 * 1024 * 1024;
        private static readonly int CHUNK_COPY_BUFFER_SIZE_IN_BYTES = 4 * 1024;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(4);
        private static readonly List<int> DEFAULT_RETRY_DELAYS = Enumerable.Repeat(200, 5).Concat(Enumerable.Repeat(1000, 29)).ToList();

        /// <inheritdoc/>
        public IList<int> RetryDelays { get; set; }

        private readonly ProKnowApi _proKnow;
        private readonly ILogger _logger;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        private static readonly IList<string> _terminalStatuses = new List<string>() { "completed", "pending", "failed" };

        /// <summary>
        /// Constructs an Uploads object
        /// </summary>
        /// <param name="proKnow">Root object for interfacing with the ProKnow API</param>
        internal Uploads(ProKnowApi proKnow)
        {
            _logger = ProKnowLogging.CreateLogger(typeof(Uploads).FullName);
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
        public async Task<UploadProcessingResults> GetUploadProcessingResultsAsync(WorkspaceItem workspace, IList<UploadResult> uploadResults)
        {
            // Create the collection of processing results for the provided set of uploads
            var idToUploadProcessingResult = new Dictionary<string, UploadProcessingResult>();

            // Create the collection of unresolved uploads
            var unresolvedUploadResults = uploadResults.ToList();

            // Query parameters for upload processing results
            Dictionary<string, object> queryParameters = null;

            // Loop until all uploads are resolved or retries have been exhausted
            bool wereRetryDelaysExhausted = false;
            var retryDelayIndex = 0;
            do
            {
                // Loop until all uploads are resolved or there are no more pages of upload processing results
                bool isPossiblyAnotherPageOfResults = true;
                do
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

                                // Reset the retry delay index since progress is being made on the upload processing
                                retryDelayIndex = 0;
                            }
                        }
                    }

                    // Remove any uploads that reached terminal status from the collection of unresolved uploads
                    foreach (var resolvedUploadId in resolvedUploadIds)
                    {
                        var index = unresolvedUploadResults.FindIndex(t => t.Id == resolvedUploadId);
                        unresolvedUploadResults.RemoveAt(index);
                    }

                    // If there is possibly another page of results
                    if (isPossiblyAnotherPageOfResults = thisPageUploadProcessingResults.Count > 0)
                    {
                        // Get the last upload on the current page
                        var lastUploadProcessingResult = thisPageUploadProcessingResults.Last();

                        // Update the query parameters to search for processed uploads updated after this one
                        if (queryParameters == null)
                        {
                            queryParameters = new Dictionary<string, object>();
                        }
                        queryParameters["updated"] = lastUploadProcessingResult.UpdatedAt;
                        queryParameters["after"] = lastUploadProcessingResult.Id;
                    }
                }
                while (unresolvedUploadResults.Count() > 0 && isPossiblyAnotherPageOfResults);

                // If retries have not been exhausted
                if (retryDelayIndex < RetryDelays.Count)
                {
                    // Give the uploads some time to process and retry
                    await Task.Delay(RetryDelays[retryDelayIndex++]);
                }
                else
                {
                    wereRetryDelaysExhausted = true;
                }
            }
            while (unresolvedUploadResults.Count() > 0 && !wereRetryDelaysExhausted);

            return new UploadProcessingResults() {
                Results = idToUploadProcessingResult.Values.ToList(),
                WereRetryDelaysExhausted = wereRetryDelaysExhausted,
                TotalRetryDelayInMsec = RetryDelays.Sum()
            };
        }

        /// <inheritdoc/>
        public async Task<UploadProcessingResults> GetUploadProcessingResultsAsync(string workspace, IList<UploadResult> uploadResults)
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
                    var uploadResult = await UploadFileAsync(workspaceId, path, overrides);
                    uploadResults.Add(uploadResult);
                }));
            }
            await Task.WhenAll(tasks);
            return uploadResults.ToList();
        }

        /// <summary>
        /// Uploads a file asynchronously
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="path">The full path to the file to be uploaded</param>
        /// <param name="overrides">Optional overrides to be applied after the file is uploaded</param>
        /// <returns>The upload result</returns>
        private async Task<UploadResult> UploadFileAsync(string workspaceId, string path, UploadFileOverrides overrides = null)
        {
            // Gather information for the upload
            var initiateFileUploadInfo = BuildInitiateFileUploadInfo(workspaceId, path, overrides);

            // Initiate the file upload
            _logger.LogDebug($"Initiating upload of file {path} with size {initiateFileUploadInfo.FileSize / 1024.0:0.##} KB.");
            var initiateFileUploadResponse = await InitiateFileUploadAsync(initiateFileUploadInfo);

            // If the file has not already been uploaded
            if (initiateFileUploadResponse.Status == "uploading")
            {
                // Upload the file contents in chunks
                var uploadChunkInfos = ChunkFile(initiateFileUploadInfo, initiateFileUploadResponse);
                _logger.LogDebug($"Uploading file {path} in {uploadChunkInfos.Count} chunk(s).");
                var tasks = new List<Task>();
                foreach (var uploadChunkInfo in uploadChunkInfos)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await UploadChunkAsync(uploadChunkInfo);
                    }
                    ));
                    
                }
                await Task.WhenAll(tasks);
            }
            else
            {
                // Overwrite the filename in case of duplicate content (ProKnow returns the original filename rather than the one just uploaded)
                initiateFileUploadResponse.Path = path;
                _logger.LogDebug($"Skipped uploading of file {path} (duplicate content).");
            }

            return new UploadResult(initiateFileUploadResponse.Id, initiateFileUploadResponse.Path, initiateFileUploadResponse.Status);
        }

        /// <summary>
        /// Collects the information needed to build an initiate file upload request
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID of the workspace to which the file will be uploaded</param>
        /// <param name="path"></param>
        /// <param name="overrides"></param>
        /// <returns>The information needed to build an initiate file upload request</returns>
        private InitiateFileUploadInfo BuildInitiateFileUploadInfo(string workspaceId, string path, UploadFileOverrides overrides = null)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return new InitiateFileUploadInfo
                    {
                        WorkspaceId = workspaceId,
                        Path = path,
                        FileSize = stream.Length,
                        Checksum = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower(),
                        ChunkSize = UPLOAD_CHUNK_SIZE_IN_BYTES,
                        NumberOfChunks = Math.Max(1, Convert.ToInt32(Math.Floor(Convert.ToDouble(stream.Length) / UPLOAD_CHUNK_SIZE_IN_BYTES))),
                        Overrides = overrides
                    };
                }
            }
        }

        /// <summary>
        /// Initiates a file upload asynchronously
        /// </summary>
        /// <param name="initiateFileUploadInfo">The information needed to build an initiate file upload request</param>
        /// <returns>The response from the file upload initiation</returns>
        private async Task<InitiateFileUploadResponse> InitiateFileUploadAsync(InitiateFileUploadInfo initiateFileUploadInfo)
        {
            var requestBody = BuildInitiateFileUploadRequestBody(initiateFileUploadInfo);
            var json = await _proKnow.Requestor.PostAsync($"/workspaces/{initiateFileUploadInfo.WorkspaceId}/uploads/", null, requestBody);
            return JsonSerializer.Deserialize<InitiateFileUploadResponse>(json);
        }

        /// <summary>
        /// Builds the body for an initiate file upload request
        /// </summary>
        /// <param name="initiateFileUploadInfo">The information to initiate the file upload</param>
        /// <returns>The content for the body</returns>
        private HttpContent BuildInitiateFileUploadRequestBody(InitiateFileUploadInfo initiateFileUploadInfo)
        {
            var uploadFileRequest = new InitiateFileUploadRequestBody
            {
                Checksum = initiateFileUploadInfo.Checksum,
                Path = initiateFileUploadInfo.Path,
                Filesize = initiateFileUploadInfo.FileSize,
                IsMultipart = initiateFileUploadInfo.NumberOfChunks > 1,
                Overrides = initiateFileUploadInfo.Overrides
            };
            var json = JsonSerializer.Serialize(uploadFileRequest, _jsonSerializerOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Split a file into chunks
        /// </summary>
        /// <param name="initiateFileUploadInfo">The information used to initiate a file upload</param>
        /// <param name="initiateFileUploadResponse">The response to the request to initiate a file upload</param>
        /// <returns>Information to upload each chunk</returns>
        private IList<UploadChunkInfo> ChunkFile(InitiateFileUploadInfo initiateFileUploadInfo,
            InitiateFileUploadResponse initiateFileUploadResponse)
        {
            var chunks = new List<UploadChunkInfo>();
            using (var inputFileStream = File.OpenRead(initiateFileUploadInfo.Path))
            {
                for (int chunkIndex = 0; chunkIndex < initiateFileUploadInfo.NumberOfChunks; chunkIndex++)
                {
                    string chunkPath;
                    long chunkSize;
                    if (initiateFileUploadInfo.NumberOfChunks > 1)
                    {
                        chunkPath = Path.GetTempFileName();

                        // When chunking, chunk size can NOT be smaller than UPLOAD_CHUNK_SIZE_IN_BYTES, i.e., the last chunk will
                        // be between UPLOAD_CHUNK_SIZE_IN_BYTES and 2 * UPLOAD_CHUNK_SIZE_IN_BYTES
                        chunkSize = chunkIndex < initiateFileUploadInfo.NumberOfChunks - 1 ? UPLOAD_CHUNK_SIZE_IN_BYTES :
                            initiateFileUploadInfo.FileSize - chunkIndex * UPLOAD_CHUNK_SIZE_IN_BYTES;

                        using (var outputFileStream = File.OpenWrite(chunkPath))
                        {
                            var totalNumberOfBytesCopied = CopyStream(inputFileStream, outputFileStream, chunkSize);
                            if (totalNumberOfBytesCopied != chunkSize)
                            {
                                var message = $"Error creating chunk {chunkIndex + 1} of {initiateFileUploadInfo.NumberOfChunks} for file {initiateFileUploadInfo.Path}.  Only {totalNumberOfBytesCopied} of {chunkSize} bytes were copied.";
                                _logger.LogError(message);
                                throw new ProKnowException(message);
                            }
                        }
                    }
                    else
                    {
                        chunkPath = initiateFileUploadInfo.Path;
                        chunkSize = initiateFileUploadInfo.FileSize;
                    }
                    var chunk = new UploadChunkInfo
                    {
                        InitiateFileUploadInfo = initiateFileUploadInfo,
                        InitiateFileUploadResponse = initiateFileUploadResponse,
                        ChunkIndex = chunkIndex,
                        ChunkPath = chunkPath,
                        ChunkSize = chunkSize
                    };
                    chunks.Add(chunk);
                }
            }

            return chunks;
        }

        /// <summary>
        /// Copies bytes from the current position in an input stream to the beginning of an output stream, advancing the
        /// position of the input stream
        /// </summary>
        /// <param name="input">The input stream</param>
        /// <param name="output">The output stream</param>
        /// <param name="numberOfBytesToCopy">The number of bytes to copy</param>
        /// <returns>The total number of bytes copied</returns>
        private long CopyStream(Stream input, Stream output, long numberOfBytesToCopy)
        {
            long totalNumberOfBytesCopied = 0L;
            byte[] buffer = new byte[CHUNK_COPY_BUFFER_SIZE_IN_BYTES];
            do
            {
                var thisNumberOfBytesToCopy = Math.Min(numberOfBytesToCopy - totalNumberOfBytesCopied, buffer.Length);
                var thisNumberOfBytesCopied = input.Read(buffer, 0, (int)thisNumberOfBytesToCopy);
                if (thisNumberOfBytesCopied == 0)
                {
                    break; // end of input stream
                }
                output.Write(buffer, 0, thisNumberOfBytesCopied);
                totalNumberOfBytesCopied += thisNumberOfBytesCopied;
            } while (totalNumberOfBytesCopied < numberOfBytesToCopy);
            return totalNumberOfBytesCopied;
        }

        /// <summary>
        /// Uploads a chunk of a file
        /// </summary>
        /// <param name="uploadChunkInfo">The information to upload a chunk</param>
        private async Task UploadChunkAsync(UploadChunkInfo uploadChunkInfo)
        {
            await _semaphore.WaitAsync();

            try
            {
                var headerKeyValuePairs = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string>("Proknow-Key", uploadChunkInfo.InitiateFileUploadResponse.Key) };
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent((uploadChunkInfo.ChunkIndex + 1).ToString()), "flowChunkNumber");
                    content.Add(new StringContent(UPLOAD_CHUNK_SIZE_IN_BYTES.ToString()), "flowChunkSize");
                    content.Add(new StringContent(uploadChunkInfo.ChunkSize.ToString()), "flowCurrentChunkSize");
                    content.Add(new StringContent(uploadChunkInfo.InitiateFileUploadInfo.NumberOfChunks.ToString()), "flowTotalChunks");
                    content.Add(new StringContent(uploadChunkInfo.InitiateFileUploadInfo.FileSize.ToString()), "flowTotalSize");
                    content.Add(new StringContent(uploadChunkInfo.InitiateFileUploadResponse.Identifier), "flowIdentifier");
                    content.Add(new StringContent(uploadChunkInfo.InitiateFileUploadInfo.Path), "flowFilename");
                    content.Add(new StringContent((uploadChunkInfo.InitiateFileUploadInfo.NumberOfChunks > 1).ToString().ToLower()), "flowMultipart");
                    using (var fs = File.OpenRead(uploadChunkInfo.ChunkPath))
                    {
                        content.Add(new StreamContent(fs), "file", uploadChunkInfo.InitiateFileUploadInfo.Path);
                        var response = await _proKnow.Requestor.PostAsync("/uploads/chunks", headerKeyValuePairs, content);
                        _logger.LogDebug($"Uploaded file {uploadChunkInfo.InitiateFileUploadInfo.Path} chunk {uploadChunkInfo.ChunkIndex + 1} of {uploadChunkInfo.InitiateFileUploadInfo.NumberOfChunks}.  Response: {response}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading chunk {uploadChunkInfo.ChunkIndex + 1} of {uploadChunkInfo.InitiateFileUploadInfo.NumberOfChunks} for file {uploadChunkInfo.InitiateFileUploadInfo.Path}.  {ex}");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
