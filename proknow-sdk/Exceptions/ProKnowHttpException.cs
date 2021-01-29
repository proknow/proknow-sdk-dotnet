using System;
using System.Runtime.Serialization;

namespace ProKnow.Exceptions
{
    /// <summary>
    /// An exception produced at the HTTP layer
    /// </summary>
    [Serializable()]
    public class ProKnowHttpException : ProKnowException
    {
        private const string _defaultMessage = "Exception occurred making HTTP request.";

        /// <summary>
        /// The HTTP method
        /// </summary>
        public string RequestMethod { get; private set; }

        /// <summary>
        /// The HTTP request URI
        /// </summary>
        public string RequestUri { get; private set; }

        /// <summary>
        /// The HTTP response status code
        /// </summary>
        public string ResponseStatusCode { get; private set; }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        /// <param name="requestVerb">The request verb (GET, POST, PUT, PATCH, DELETE)</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="responseStatusCode">The response code</param>
        /// <param name="message">The message</param>
        /// <param name="innerException">The exception that caused the current exception</param>
        public ProKnowHttpException(string requestVerb, string requestUri, string responseStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            RequestMethod = requestVerb;
            RequestUri = requestUri;
            ResponseStatusCode = responseStatusCode;
        }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        /// <param name="requestVerb">The request verb (GET, POST, PUT, PATCH, DELETE)</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="responseStatusCode">The response code</param>
        /// <param name="message">The message</param>
        public ProKnowHttpException(string requestVerb, string requestUri, string responseStatusCode, string message)
            : base(message)
        {
            RequestMethod = requestVerb;
            RequestUri = requestUri;
            ResponseStatusCode = responseStatusCode;
        }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        /// <param name="requestVerb">The request verb (GET, POST, PUT, PATCH, DELETE)</param>
        /// <param name="requestUri">The request URI</param>
        /// <param name="responseStatusCode">The response code</param>
        public ProKnowHttpException(string requestVerb, string requestUri, string responseStatusCode)
            : base(_defaultMessage)
        {
            RequestMethod = requestVerb;
            RequestUri = requestUri;
            ResponseStatusCode = responseStatusCode;
        }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        public ProKnowHttpException() : base(_defaultMessage)
        {
        }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        /// <param name="message">The message</param>
        public ProKnowHttpException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The exception that caused the current exception</param>
        public ProKnowHttpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructs a ProKnowHttpException object
        /// </summary>
        /// <param name="info">The serialization information</param>
        /// <param name="context">The streaming context</param>
        protected ProKnowHttpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
