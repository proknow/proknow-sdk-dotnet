using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ProKnow.Tools
{
    /// <summary>
    /// Provides tools for handling JSON
    /// </summary>
    public static class JsonTools
    {
        /// <summary>
        /// Extracts a boolean value from a set of properties
        /// </summary>
        /// <param name="data">A set of properties (key-value pairs)</param>
        /// <param name="key">The key for the desired property</param>
        /// <param name="defaultValue">The default value for the property if the property is not present</param>
        /// <returns>The value of the specified property, if present, otherwise the default value</returns>
        /// <exception cref="System.Text.Json.JsonException">Thrown if the property is present and does not have a boolean value</exception>
        public static bool DeserializeBoolean(Dictionary<string, object> data, string key, bool defaultValue)
        {
            if (!data.ContainsKey(key))
            {
                return defaultValue;
            }
            var jsonElement = (JsonElement)data[key];
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    throw new JsonException($"The '{key}' property does not have a boolean value.");
            }
        }

        /// <summary>
        /// Extracts a string value from a set of properties
        /// </summary>
        /// <param name="data">A set of properties (key-value pairs)</param>
        /// <param name="key">The key for the desired property</param>
        /// <returns>The value of the specified property, if present, otherwise null</returns>
        public static string DeserializeString(Dictionary<string, object> data, string key)
        {
            if (!data.ContainsKey(key))
            {
                return null;
            }
            return Convert.ToString(data[key]);
        }
    }
}
