using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProKnow
{
    /// <summary>
    /// Issues requests to the ProKnow API
    /// </summary>
    public class Requestor
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        });

        private string _baseUrl;
        private AuthenticationHeaderValue _authenticationHeaderValue;

        /// <summary>
        /// Constructs a requestor object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="id">The ID from the ProKnow credentials JSON file</param>
        /// <param name="secret">The secret from the ProKnow credentials JSON file</param>
        public Requestor(string baseUrl, string id, string secret)
        {
            _baseUrl = $"{baseUrl}/api";
            _authenticationHeaderValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{id}:{secret}")));
        }

        /// <summary>
        /// Issues an asynchronous HTTP DELETE request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request is not successful</exception>
        public Task<string> DeleteAsync(string route)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/{route}");
            request.Headers.Authorization = _authenticationHeaderValue;
            var httpResponseMessage = _httpClient.SendAsync(request);
            return httpResponseMessage.ContinueWith(t => HandleResponseAsync(t.Result)).Unwrap();
        }

        /// <summary>
        /// Issues an asynchronous HTTP GET request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request is not successful</exception>
        public Task<string> GetAsync(string route)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{route}");
            request.Headers.Authorization = _authenticationHeaderValue;
            var httpResponseMessage = _httpClient.SendAsync(request);
            return httpResponseMessage.ContinueWith(t => HandleResponseAsync(t.Result)).Unwrap();
        }

        /// <summary>
        /// Issues an asynchronous HTTP POST request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request is not successful</exception>
        public Task<string> PostAsync(string route, IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/{route}");
            request.Headers.Authorization = _authenticationHeaderValue;
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
            var httpResponseMessage = _httpClient.SendAsync(request);
            return httpResponseMessage.ContinueWith(t => HandleResponseAsync(t.Result)).Unwrap();
        }

        /// <summary>
        /// Issues an asynchronous HTTP GET request with a streaming response
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="path">The full path to the file to which to write the response</param>
        /// <returns>The full path to the file containing the response</returns>
        public async Task<string> StreamAsync(string route, string path)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{route}");
            request.Headers.Authorization = _authenticationHeaderValue;
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            using (var httpResponseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                using (var streamToReadFrom = await httpResponseMessage.Content.ReadAsStreamAsync())
                {
                    using (var streamToWriteTo = File.Open(path, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }
                }
            }
            return path;
        }

        /// <summary>
        /// Handles the response from an HTTP request
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request is not successful</exception>
        private Task<string> HandleResponseAsync(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return content;
        }
    }
}
