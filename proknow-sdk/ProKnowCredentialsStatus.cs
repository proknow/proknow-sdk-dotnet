using System;
using System.Net;

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
        ///  The <see cref="HttpStatusCode"/> string value
        /// </summary>
        public string ErrorCode { get; private set; }

        /// <summary>
        ///  The original <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Creates a ProKnowCredentialsStatus object
        /// </summary>
        /// <param name="isValid">The flag indicating whether the specified ProKnow credentials (API key) is valid
        /// for the ProKnow subdomain specified by the base URL</param>
        /// <param name="errorMessage">The error message or null if the credentials (API key) is invalid</param>
        /// <param name="errorCode">The <see cref="HttpStatusCode"/> string value</param>
        /// <param name="exception">The original <see cref="Exception"/></param>
        internal ProKnowCredentialsStatus(bool isValid, string errorMessage = null, string errorCode = null, Exception exception = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Exception = exception;
        }
    }
}
