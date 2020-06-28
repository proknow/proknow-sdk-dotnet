namespace ProKnow.Exceptions
{
    /// <summary>
    /// An exception produced at the HTTP layer
    /// </summary>
    public class ProKnowHttpException : ProKnowException
    {
        /// <summary>
        /// The HTTP status code
        /// </summary>
        public string StatusCode { get; private set; }

        /// <summary>
        /// Creates a ProKnowHttpException
        /// </summary>
        /// <param name="statusCode">The status code</param>
        /// <param name="body">The optional body</param>
        public ProKnowHttpException(string statusCode, string body = null)
            : base($"HttpError({statusCode}{(body != null ? $", {body}" : "")})")
        {
            StatusCode = statusCode;
        }
    }
}
