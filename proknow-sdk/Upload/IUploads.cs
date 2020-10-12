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
        /// Upload file(s) asynchronously
        /// </summary>
        /// <param name="workspaceItem">The workspace</param>
        /// <param name="path">The folder or file path</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <param name="doWait">Indicates whether to wait until all uploads reach a terminal state</param>
        /// <returns>The upload results</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <example>This example shows how to upload a directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// var workspaceItem = await pk.Workspaces.ResolveByNameAsync("Upload Test");
        /// await pk.Uploads.UploadAsync(workspaceItem, "./DICOM");
        /// </code>
        /// </example>
        /// <remarks>This overload is more performant than the one that takes a string workspace ProKnow ID or name because the
        /// workspace does not need to be resolved.</remarks>
        Task<UploadBatch> UploadAsync(WorkspaceItem workspaceItem, string path, UploadFileOverrides overrides = null, bool doWait = true);

        /// <summary>
        /// Upload file(s) asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace</param>
        /// <param name="path">The folder or file path</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <param name="doWait">Indicates whether to wait until all uploads reach a terminal state</param>
        /// <returns>The upload results</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
        /// <example>This example shows how to upload a directory of files:
        /// <code>
        /// using ProKnow;
        /// using System.Threading.Tasks;
        ///
        /// var pk = new ProKnowApi("https://example.proknow.com", "credentials.json");
        /// await pk.Uploads.UploadAsync("Upload Test", "./DICOM");
        /// </code>
        /// </example>
        /// <remarks>This overload is less performant than the one that takes a workspace item because the workspace needs to be
        /// resolved.</remarks>
        Task<UploadBatch> UploadAsync(string workspace, string path, UploadFileOverrides overrides = null, bool doWait = true);

        /// <summary>
        /// Upload files asynchronously
        /// </summary>
        /// <param name="workspaceItem">The workspace</param>
        /// <param name="paths">The folder and/or file paths</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <param name="doWait">Indicates whether to wait until all uploads reach a terminal state</param>
        /// <returns>The upload results</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
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
        /// await pk.Uploads.UploadAsync(workspaceItem, paths);
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
        /// await pk.Uploads.UploadAsync("Upload Test", paths);
        /// </code>
        /// </example>
        /// <remarks>This overload is more performant than the one that takes a string workspace ProKnow ID or name because the
        /// workspace does not need to be resolved.</remarks>
        Task<UploadBatch> UploadAsync(WorkspaceItem workspaceItem, IList<string> paths, UploadFileOverrides overrides = null, bool doWait = true);

        /// <summary>
        /// Upload files asynchronously
        /// </summary>
        /// <param name="workspace">The ProKnow ID or name of the workspace</param>
        /// <param name="paths">The folder and/or file paths</param>
        /// <param name="overrides">Optional overrides to be applied after the files are uploaded</param>
        /// <param name="doWait">Indicates whether to wait until all uploads reach a terminal state</param>
        /// <returns>The upload results</returns>
        /// <exception cref="ProKnowWorkspaceLookupException">If no matching workspace was found</exception>
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
        /// await pk.Uploads.UploadAsync("Upload Test", paths);
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
        /// await pk.Uploads.UploadAsync("Upload Test", paths);
        /// </code>
        /// </example>
        /// <remarks>This overload is less performant than the one that takes a workspace item because the workspace needs to be
        /// resolved.</remarks>
        Task<UploadBatch> UploadAsync(string workspace, IList<string> paths, UploadFileOverrides overrides = null, bool doWait = true);
    }
}
