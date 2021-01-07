using ProKnow.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProKnow.Upload
{
    /// <summary>
    /// Interacts with uploads in ProKnow organization
    /// </summary>
    public interface IUploads
    {
        /// <summary>
        /// The retry delays in milliseconds for obtaining ProKnow upload processing results
        /// </summary>
        IList<int> RetryDelays { get; set; }

        /// <summary>
        /// Upload file(s) asynchronously
        /// </summary>
        /// <param name="workspaceItem">The workspace</param>
        /// <param name="path">The folder or file path</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <returns>The upload results</returns>
        /// <remarks>
        /// <para>NOTE that this task completes when the upload itself has finished.  ProKnow still needs to process each object
        /// uploaded before it becomes available for use.  <see cref="GetUploadProcessingResultsAsync(WorkspaceItem, IList{UploadResult})"/> 
        /// to obtain the processing results.</para>
        /// <para>This overload is more performant than the one that takes a string workspace ProKnow ID or name because the
        /// workspace does not need to be resolved.</para>
        /// </remarks>
        /// <example>This example shows how to upload a directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// </code>
        /// </example>
        Task<IList<UploadResult>> UploadAsync(WorkspaceItem workspaceItem, string path, UploadFileOverrides overrides = null);

        /// <summary>
        /// Upload file(s) asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace</param>
        /// <param name="path">The folder or file path</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <returns>The upload results</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <remarks>
        /// <para>NOTE that this task completes when the upload itself has finished.  ProKnow still needs to process each object
        /// uploaded before it becomes available for use.  <see cref="GetUploadProcessingResultsAsync(string, IList{UploadResult})"/> 
        /// to obtain the processing results.</para>
        /// <para>This overload is less performant than the one that takes a workspace item because the workspace needs to be
        /// resolved.</para>
        /// </remarks>
        /// <example>This example shows how to upload a directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var uploadResults = await pk.Uploads.UploadAsync("Upload Test", "./DICOM");
        /// </code>
        /// </example>
        Task<IList<UploadResult>> UploadAsync(string workspace, string path, UploadFileOverrides overrides = null);

        /// <summary>
        /// Upload files asynchronously
        /// </summary>
        /// <param name="workspaceItem">The workspace</param>
        /// <param name="paths">The folder and/or file paths</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <returns>The upload results</returns>
        /// <remarks>
        /// <para>NOTE that this task completes when the upload itself has finished.  ProKnow still needs to process each object
        /// uploaded before it becomes available for use.  <see cref="GetUploadProcessingResultsAsync(WorkspaceItem, IList{UploadResult})"/> 
        /// to obtain the processing results.</para>
        /// <para>This overload is more performant than the one that takes a string workspace ProKnow ID or name because the
        /// workspace does not need to be resolved.</para>
        /// </remarks>
        /// <example>This example shows how to upload a directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var paths = new List&lt;string&gt;() {
        ///     Path.Join("DICOM", "img000001.dcm"),
        ///     Path.Join("DICOM", "img000002.dcm"),
        ///     Path.Join("DICOM", "img000003.dcm"),
        ///     Path.Join("DICOM", "img000004.dcm"),
        ///     Path.Join("DICOM", "img000005.dcm")
        /// }
        /// var workspaceItem = await pk.Workspaces.ResolvedByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, paths);
        /// </code>
        /// </example>
        /// <example>Lists containing both directories and file paths are also permitted:
        /// <code>
        /// using ProKnow;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var paths = new List&lt;string&gt;() {
        ///     Path.Join("DICOM", "CT"),
        ///     Path.Join("DICOM", "structures.dcm"),
        ///     Path.Join("DICOM", "plan.dcm"),
        ///     Path.Join("DICOM", "dose.dcm")
        /// }
        /// var workspaceItem = await pk.Workspaces.ResolvedByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, paths);
        /// </code>
        /// </example>
        Task<IList<UploadResult>> UploadAsync(WorkspaceItem workspaceItem, IList<string> paths, UploadFileOverrides overrides = null);

        /// <summary>
        /// Upload files asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace</param>
        /// <param name="paths">The folder and/or file paths</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <returns>The upload results</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <remarks>
        /// <para>NOTE that this task completes when the upload itself has finished.  ProKnow still needs to process each object
        /// uploaded before it becomes available for use.  <see cref="GetUploadProcessingResultsAsync(string, IList{UploadResult})"/> 
        /// to obtain the processing results.</para>
        /// <para>This overload is less performant than the one that takes a workspace item because the workspace needs to be
        /// resolved.</para>
        /// </remarks>
        /// <example>This example shows how to upload a directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var paths = new List&lt;string&gt;() {
        ///     Path.Join("DICOM", "img000001.dcm"),
        ///     Path.Join("DICOM", "img000002.dcm"),
        ///     Path.Join("DICOM", "img000003.dcm"),
        ///     Path.Join("DICOM", "img000004.dcm"),
        ///     Path.Join("DICOM", "img000005.dcm")
        /// }
        /// var uploadResults = await pk.Uploads.UploadAsync("Upload Test", paths);
        /// </code>
        /// </example>
        /// <example>Lists containing both directories and file paths are also permitted:
        /// <code>
        /// using ProKnow;
        /// using System.Collections.Generic;
        /// using System.IO;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var paths = new List&lt;string&gt;() {
        ///     Path.Join("DICOM", "CT"),
        ///     Path.Join("DICOM", "structures.dcm"),
        ///     Path.Join("DICOM", "plan.dcm"),
        ///     Path.Join("DICOM", "dose.dcm")
        /// }
        /// var uploadResults = await pk.Uploads.UploadAsync("Upload Test", paths);
        /// </code>
        /// </example>
        Task<IList<UploadResult>> UploadAsync(string workspace, IList<string> paths, UploadFileOverrides overrides = null);

        /// <summary>
        /// Gets the processing results for a provided set of uploads
        /// </summary>
        /// <param name="workspace">The ProKnow ID for the workspace</param>
        /// <param name="uploadResults">The upload results for each file</param>
        /// <returns>The processing results for each file</returns>
        /// <remarks>
        /// <para>The Results property of the return value of this method can be used to construct an <see cref="UploadBatch"/>
        /// which can be used to look up patients, entities, spatial registration objects, and file statuses within the processing
        /// results.</para>
        /// <para>This overload is more performant than the one that takes a string workspace ProKnow ID or name because the
        /// workspace does not need to be resolved.</para>
        /// </remarks>
        /// <example>This example shows how to get the processing results for an uploaded directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// var uploadResults = await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync(workspaceItem, uploadResults);
        /// </code>
        /// </example>
        Task<UploadProcessingResults> GetUploadProcessingResultsAsync(WorkspaceItem workspace, IList<UploadResult> uploadResults);

        /// <summary>
        /// Gets the processing results for a provided set of uploads
        /// </summary>
        /// <param name="workspaceId">The ProKnow ID for the workspace</param>
        /// <param name="uploadResults">The upload results for each file</param>
        /// <returns>The processing results for each file</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <remarks>
        /// <para>The Results property of the return value of this method can be used to construct an <see cref="UploadBatch"/>
        /// which can be used to look up patients, entities, spatial registration objects, and file statuses within the processing
        /// results.</para>
        /// <para>This overload is less performant than the one that takes a workspace item because the workspace needs to be
        /// resolved.</para>
        /// </remarks>
        /// <example>This example shows how to get the processing results for an uploaded directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var uploadResults = await pk.Uploads.UploadAsync("Upload Test", "./DICOM");
        /// var uploadProcessingResults = await pk.Uploads.GetUploadProcessingResultsAsync("Upload Test", uploadResults);
        /// </code>
        /// </example>
        Task<UploadProcessingResults> GetUploadProcessingResultsAsync(string workspaceId, IList<UploadResult> uploadResults);
    }
}
