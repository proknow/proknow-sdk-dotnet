using Microsoft.Extensions.Logging;

namespace ProKnow
{
    /// <summary>
    /// Provides the logger factory for the .NET SDK
    /// </summary>
    public class ProKnowLogging
    {
        private static ILoggerFactory _loggerFactory = null;

        /// <summary>
        /// The logger factory
        /// </summary>
        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_loggerFactory == null)
                {
                    _loggerFactory = new LoggerFactory();
                }
                return _loggerFactory;
            }
            set
            {
                _loggerFactory = value;
            }
        }

        /// <summary>
        /// Creates a logger
        /// </summary>
        /// <param name="categoryName">The category name</param>
        /// <returns>The created logger</returns>
        public static ILogger CreateLogger(string categoryName)
        {
            return LoggerFactory.CreateLogger(categoryName);
        }
    }
}
