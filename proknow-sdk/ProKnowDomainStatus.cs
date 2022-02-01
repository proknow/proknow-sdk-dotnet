namespace ProKnow
{
    /// <summary>
    /// Provides the ProKnow domain status
    /// </summary>
    public class ProKnowDomainStatus
    {
        /// <summary>
        /// The flag indicating whether the ProKnow domain is up and reachable
        /// </summary>
        public bool IsOk { get; private set; }

        /// <summary>
        /// The error message or null if the ProKnow domain is up and reachable
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Creates a ProKnowDomainStatus object
        /// </summary>
        /// <param name="isOk">The flag indicating whether the ProKnow domain is up and reachable</param>
        /// <param name="errorMessage">The error message or null if the ProKnow domain is up and reachable</param>
        internal ProKnowDomainStatus(bool isOk, string errorMessage = null)
        {
            IsOk = isOk;
            ErrorMessage = errorMessage;
        }
    }
}
