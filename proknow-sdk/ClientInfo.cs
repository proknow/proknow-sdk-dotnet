using System;

namespace ProKnow
{
    /// <summary>
    /// Holds the name and version of the calling client process
    /// </summary>
    public class ClientInfo
    {
        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string Version { get; set; }

        /// <summary>
        /// Constructs a client info object.
        /// </summary>
        /// <param name="name">The client name of the calling process</param>
        /// <param name="version">The client version of the calling process</param>
        public ClientInfo(string name, string version)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The 'name' parameter must be provided.");
            }
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException("The 'name' parameter must be provided.");
            }
            Name = name;
            Version = version;
        }
    }
}
