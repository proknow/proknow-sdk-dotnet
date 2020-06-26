namespace ProKnow.Exceptions
{
    /// <summary>
    /// An exception that occurs when looking up a workspace
    /// </summary>
    public class ProKnowWorkspaceLookupException : ProKnowException
    {
        /// <summary>
        /// Creates a ProKnowWorkspaceLookupException object
        /// </summary>
        /// <param name="message">The exception message</param>
        public ProKnowWorkspaceLookupException(string message)
            : base(message)
        {
        }
    }
}
