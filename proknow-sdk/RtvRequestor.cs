using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ProKnow.Exceptions;

namespace ProKnow
{
    /// <summary>
    /// Issues requests to the RTV API
    /// </summary>
    internal class RtvRequestor
    {
        #region Enums
        public enum ObjectType
        {
            Unknown,
            ImageSet,
            StructureSet,
            Plan,
            Dose
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// List of headers to include in all requests.
        /// </summary>
        public IList<KeyValuePair<string, string>> DefaultHeaders { get; set; }

        public Dictionary<ObjectType, string> ApiVersions = new Dictionary<ObjectType, string>();
        #endregion

        #region Private Properties
        private readonly string _baseUrl;

        private string _rtvUrl { get; set; }

        private static HttpClient _httpClient = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });
        #endregion

        /// <summary>
        /// Constructs a Rtv Requestor object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="defaultHeaders">Optional list of headers to include in all requests</param>
        public RtvRequestor(string baseUrl, IList<KeyValuePair<string, string>> defaultHeaders = null)
        {
            if (String.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("The 'baseUrl' parameter must be provided.");
            }
            DefaultHeaders = defaultHeaders;
            _baseUrl = baseUrl;
        }

        #region Public Methods
        /// <summary>
        /// Issues an asynchronous HTTP GET request that expects a binary response
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <returns>A task that returns the response as a byte array</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        public async Task<byte[]> GetBinaryAsync(string route, IList<KeyValuePair<string, string>> headerKeyValuePairs = null,
            Dictionary<string, object> queryParameters = null)
        {
            return await MakeRequestForBinaryResponse(HttpMethod.Get, route, queryParameters, headerKeyValuePairs);
        }

        /// <summary>
        /// Issues an asynchronous HTTP POST request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        public async Task<string> PostAsync(string route, IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            return await MakeRequestForStringResponse(HttpMethod.Post, route, queryParameters: null, headerKeyValuePairs, content);
        }
        #endregion

        #region Internal Methods
        /// <summary>
        /// Gets the API version for a given object type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>API version</returns>
        internal async Task<string> GetApiVersion(ObjectType type)
        {
            if (ApiVersions.Count > 0)
            {
                return ApiVersions[type];
            }
            var response = await MakeRequestForStringResponse(HttpMethod.Get, $"/status");
            var responseJson = JsonSerializer.Deserialize<JsonElement>(response).GetProperty("api_version");
            ApiVersions.Add(ObjectType.ImageSet, responseJson.GetProperty("imageset").GetString());
            ApiVersions.Add(ObjectType.StructureSet, responseJson.GetProperty("structureset").GetString());
            ApiVersions.Add(ObjectType.Plan, responseJson.GetProperty("plan").GetString());
            ApiVersions.Add(ObjectType.Dose, responseJson.GetProperty("dose").GetString());
            return ApiVersions[type];
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets a Prefix for rtv route with source
        /// </summary>
        private async Task GetPrefix()
        {
            if (_rtvUrl == null)
            {
                var response = await MakeRequest(HttpMethod.Get, _baseUrl + "/ui/variables.js");
                var responseContent = string.Empty;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }
                Match match = Regex.Match(responseContent, @"\""rtVisualizerSourceName\"":\s*\""([^,]+)\""");
                if (match.Success)
                {
                    string source = match.Groups[1].Value;
                    _rtvUrl = _baseUrl + "/rtv/" + source;
                }
                else
                {
                    throw new Exception("RTV Source not found");
                }
            }
        }

        /// <summary>
        /// Makes an HTTP request that will return a string response
        /// </summary>
        /// <param name="method">The HTTP method</param>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>The string response</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        private async Task<string> MakeRequestForStringResponse(HttpMethod method, string route, Dictionary<string, object> queryParameters = null,
            IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            await GetPrefix();
            var response = await MakeRequest(method, _rtvUrl + route, queryParameters, headerKeyValuePairs, content);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Makes an HTTP request that will return a binary response
        /// </summary>
        /// <param name="method">The HTTP method</param>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>The binary response</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        private async Task<Byte[]> MakeRequestForBinaryResponse(HttpMethod method, string route, Dictionary<string, object> queryParameters = null,
            IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            await GetPrefix();
            var response = await MakeRequest(method, _rtvUrl + route, queryParameters, headerKeyValuePairs, content);
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// Makes an HTTP request
        /// </summary>
        /// <param name="method">The HTTP method</param>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>The response</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        private async Task<HttpResponseMessage> MakeRequest(HttpMethod method, string route, Dictionary<string, object> queryParameters = null,
            IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            var request = new HttpRequestMessage(method, BuildUriString($"{route}", queryParameters));
            try
            {
                if (DefaultHeaders != null)
                {
                    foreach (var kvp in DefaultHeaders)
                    {
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }
                }
                if (headerKeyValuePairs != null)
                {
                    foreach (var kvp in headerKeyValuePairs)
                    {
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }
                }
                if (content != null)
                {
                    request.Content = content;
                }
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.Content != null)
                    {
                        // Response content contains error message from ProKnow
                        throw new ProKnowHttpException(request.Method.ToString(), request.RequestUri.ToString(), response.StatusCode.ToString(), await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        throw new ProKnowHttpException(request.Method.ToString(), request.RequestUri.ToString(), response.StatusCode.ToString());
                    }
                }
                return response;
            }
            catch (ProKnowHttpException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ProKnowHttpException(request.Method.ToString(), request.RequestUri.ToString(), HttpStatusCode.BadRequest.ToString(), "Exception occurred making HTTP request.", ex);
            }
        }

        /// <summary>
        /// Builds a URI string
        /// </summary>
        /// <param name="route">The route</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <returns>A URI string</returns>
        private string BuildUriString(string route, Dictionary<string, object> queryParameters = null)
        {
            var uri = new UriBuilder(route);
            if (queryParameters != null)
            {
                foreach (var queryParameter in queryParameters)
                {
                    var queryToAppend = $"{queryParameter.Key}={queryParameter.Value}";
                    if (uri.Query != null && uri.Query.Length > 1)
                    {
                        uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
                    }
                    else
                    {
                        uri.Query = queryToAppend;
                    }
                }
            }
            return uri.ToString();
        }
        #endregion
    }
}
