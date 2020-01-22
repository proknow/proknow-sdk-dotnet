using System;
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
        private static readonly HttpClient _httpClient = new HttpClient();

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
        /// Issues an asynchronous HTTP GET request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request is not successful</exception>
        public Task<string> getAsync(string route)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{route}");
            request.Headers.Authorization = _authenticationHeaderValue;
            var httpResponseMessage = _httpClient.SendAsync(request);
            return httpResponseMessage.ContinueWith(t => HandleResponseAsync(t.Result)).Unwrap();
        }

        /// <summary>
        /// Handles the response from an HTTP request
        /// </summary>
        /// <param name="response">The response</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="System.Net.Http.HttpRequestException">Thrown when the HTTP request is not successful</exception>
        private Task<string> HandleResponseAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync();
        }
    }
}
