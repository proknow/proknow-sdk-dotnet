namespace ProKnow
{
    /// <summary>
    /// Provides the status of the ProKnow API connection
    /// </summary>
    public class ProKnowConnectionStatus
    {
        /// <summary>
        /// The flag indicating whether the connection is valid
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// The error message or null if the connection is valid
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Creates a ProKnowConnectionStatus object
        /// </summary>
        /// <param name="isValid">The flag indicating whether the connection is valid</param>
        /// <param name="errorMessage">The error message or null if the connection is valid</param>
        internal ProKnowConnectionStatus(bool isValid, string errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
