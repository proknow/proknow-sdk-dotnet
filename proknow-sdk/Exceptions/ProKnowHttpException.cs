namespace ProKnow.Exceptions
{
    /// <summary>
    /// An exception produced at the HTTP layer
    /// </summary>
    public class ProKnowHttpException : ProKnowException
    {
        /// <summary>
        /// Creates a ProKnowHttpException
        /// </summary>
        /// <param name="statusCode">The status code</param>
        /// <param name="body">The body</param>
        public ProKnowHttpException(string statusCode, string body)
            : base($"HttpError({statusCode}, {body})")
        {
        }
    }
}
