namespace ProKnow.Exceptions
{
    /// <summary>
    /// Indicates that the operation is not valid for the current object or with the given inputs
    /// </summary>
    public class InvalidOperationError : ProKnowException
    {
        /// <summary>
        /// Creates an InvalidOperationError object
        /// </summary>
        /// <param name="message">The exception message</param>
        public InvalidOperationError(string message)
            : base(message)
        {
        }
    }
}
