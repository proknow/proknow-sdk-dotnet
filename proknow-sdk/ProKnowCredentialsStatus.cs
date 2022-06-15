namespace ProKnow
{
    /// <summary>
    /// Provides the ProKnow credentials (API key) status
    /// </summary>
    public class ProKnowCredentialsStatus
    {
        /// <summary>
        /// The flag indicating whether the specified ProKnow credentials (API key) is valid for the ProKnow
        /// subdomain specified by the base URL
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// The error message or null if the credentials (API key) is invalid
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// The response error code
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        /// Creates a ProKnowCredentialsStatus object
        /// </summary>
        /// <param name="isValid">The flag indicating whether the specified ProKnow credentials (API key) is valid
        /// for the ProKnow subdomain specified by the base URL</param>
        /// <param name="errorMessage">The error message or null if the credentials (API key) is invalid</param>
        /// <param name="errorCode">The response error code</param>
        internal ProKnowCredentialsStatus(bool isValid, string errorMessage = null, string errorCode = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
}
