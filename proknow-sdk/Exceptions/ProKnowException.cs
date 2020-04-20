using System;

namespace ProKnow.Exceptions
{
    /// <summary>
    /// The base class for exceptions produced in this SDK
    /// </summary>
    public class ProKnowException : Exception
    {
        /// <summary>
        /// Creates a ProKnowException object
        /// </summary>
        /// <param name="message">The exception message</param>
        public ProKnowException(string message)
            : base(message)
        {
        }
    }
}
