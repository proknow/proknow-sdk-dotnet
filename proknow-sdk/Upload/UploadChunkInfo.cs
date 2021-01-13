namespace ProKnow.Upload
{
    /// <summary>
    /// Information used to upload a chunk
    /// </summary>
    internal class UploadChunkInfo
    {
        /// <summary>
        /// The information needed to initiate a file upload
        /// </summary>
        public InitiateFileUploadInfo InitiateFileUploadInfo { get; set; }

        /// <summary>
        /// The response from the initiation of the file upload
        /// </summary>
        public InitiateFileUploadResponse InitiateFileUploadResponse { get; set; }

        /// <summary>
        /// The index of this chunk
        /// </summary>
        public int ChunkIndex { get; set; }

        /// <summary>
        /// The path of this chunk
        /// </summary>
        public string ChunkPath { get; set; }

        /// <summary>
        /// The size in bytes of this chunk
        /// </summary>
        public long ChunkSize { get; set; }
    }
}
