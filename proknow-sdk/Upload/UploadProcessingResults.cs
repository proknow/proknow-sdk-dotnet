using System.Collections.Generic;

namespace ProKnow.Upload
{
    /// <summary>
    /// A container for upload processing results
    /// </summary>
    public class UploadProcessingResults
    {
        /// <summary>
        /// The processing results for each upload
        /// </summary>
        public IList<UploadProcessingResult> Results { get; set; }

        /// <summary>
        /// The flag indicating whether retry delays were exhausted
        /// </summary>
        public bool WereRetryDelaysExhausted { get; set; }

        /// <summary>
        /// The total retry delay in milliseconds
        /// </summary>
        public int TotalRetryDelayInMsec { get; set; }
    }
}
