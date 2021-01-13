namespace ProKnow.Upload
{
    /// <summary>
    /// Information used to initiate the upload of a file
    /// </summary>
    internal class InitiateFileUploadInfo
    {
        /// <summary>
        /// The ProKnow ID of the workspace to which the file will be uploaded
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// The full path to the file to be uploaded
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The file size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// The MD5 checksum of the file
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// The chunk size to be used for this file
        /// </summary>
        public int ChunkSize { get; set; }

        /// <summary>
        /// The number of chunks in which this file will be uploaded
        /// </summary>
        public int NumberOfChunks { get; set; }

        /// <summary>
        /// Optional overrides to be applied to the data
        /// </summary>
        public UploadFileOverrides Overrides { get; set; }
    }
}
