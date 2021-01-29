using System;
using System.Runtime.Serialization;

namespace ProKnow.Exceptions
{
    /// <summary>
    /// The base class for exceptions produced in this SDK
    /// </summary>
    [Serializable()]
    public class ProKnowException : Exception
    {
        /// <summary>
        /// The default message
        /// </summary>
        public static readonly string DefaultMessage = "ProKnow .NET SDK Exception";

        /// <summary>
        /// Constructs a ProKnowException object
        /// </summary>
        public ProKnowException() : base(DefaultMessage)
        {
        }

        /// <summary>
        /// Constructs a ProKnowException object
        /// </summary>
        /// <param name="message">The message</param>
        public ProKnowException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructs a ProKnowException object
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The exception that caused the current exception</param>
        public ProKnowException(string message, Exception innerException) : base(message, innerException)
        { 
        }

        /// <summary>
        /// Constructs a ProKnowException object
        /// </summary>
        /// <param name="info">The serialization information</param>
        /// <param name="context">The streaming context</param>
        protected ProKnowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
