﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ProKnow.Exceptions;

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

        private readonly string _baseUrl;
        private readonly AuthenticationHeaderValue _authenticationHeaderValue;

        /// <summary>
        /// Constructs a requestor object
        /// </summary>
        /// <param name="baseUrl">The base URL to ProKnow, e.g. 'https://example.proknow.com'</param>
        /// <param name="id">The ID from the ProKnow credentials JSON file</param>
        /// <param name="secret">The secret from the ProKnow credentials JSON file</param>
        public Requestor(string baseUrl, string id, string secret)
        {
            if (String.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("The 'baseUrl' parameter must be provided.");
            }
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("The 'id' parameter must be provided.");
            }
            if (String.IsNullOrWhiteSpace(secret))
            {
                throw new ArgumentException("The 'secret' parameter must be provided.");
            }
            _baseUrl = $"{baseUrl}/api";
            _authenticationHeaderValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{id}:{secret}")));
        }

        /// <summary>
        /// Issues an asynchronous HTTP DELETE request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        public async Task<string> DeleteAsync(string route, IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            return await MakeRequestForStringResponse(HttpMethod.Delete, route, queryParameters: null, headerKeyValuePairs, content);
        }

        /// <summary>
        /// Issues an asynchronous HTTP GET request that expects a string response
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        public async Task<string> GetAsync(string route, IList<KeyValuePair<string, string>> headerKeyValuePairs = null,
            Dictionary<string, object> queryParameters = null)
        {
            // With a GET request, if the base URL is malformed, it's possible that ProKnow will return 200 OK along with its index.html, hence why we request a non-HTML string response
            return await MakeRequestForNonHtmlStringResponse(HttpMethod.Get, route, queryParameters, headerKeyValuePairs);
        }

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

        /// <summary>
        /// Issues an asynchronous HTTP PUT request
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>A task that returns the response as a string</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        public async Task<string> PutAsync(string route, IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            return await MakeRequestForStringResponse(HttpMethod.Put, route, queryParameters: null, headerKeyValuePairs, content);
        }

        /// <summary>
        /// Issues an asynchronous HTTP GET request with a streaming response
        /// </summary>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="path">The full path to the file to which to write the response</param>
        /// <returns>The full path to the file containing the response</returns>
        /// <exception cref="ProKnowException">If path is to an existing directory rather than to a new or existing file</exception>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        public async Task<string> StreamAsync(string route, string path)
        {
            if (Directory.Exists(path))
            {
                throw new ProKnowException($"Cannot stream '{route}' to '{path}'.  It is a path to an existing directory.");
            }
            var parent = Directory.GetParent(path).FullName;
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{route}");
            try
            {
                request.Headers.Authorization = _authenticationHeaderValue;
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
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
                using (var streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    using (var streamToWriteTo = File.Open(path, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(streamToWriteTo);
                    }
                }
                return path;
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
            IList <KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            var response = await MakeRequest(method, route, queryParameters, headerKeyValuePairs, content);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Makes an HTTP request that will return a non-HTML string response
        /// </summary>
        /// <param name="method">The HTTP method</param>
        /// <param name="route">The API route to use in the request</param>
        /// <param name="queryParameters">Optional query parameters</param>
        /// <param name="headerKeyValuePairs">Optional key-value pairs to be included in the header</param>
        /// <param name="content">Optional content for the body</param>
        /// <returns>The non-HTML string response</returns>
        /// <exception cref="ProKnowHttpException">If the HTTP request is not successful</exception>
        private async Task<string> MakeRequestForNonHtmlStringResponse(HttpMethod method, string route, Dictionary<string, object> queryParameters = null,
            IList<KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            var response = await MakeRequest(method, route, queryParameters, headerKeyValuePairs, content);
            var responseContent = String.Empty;
            if (response.Content != null)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }
            if (response.Content.Headers.ContentType.MediaType == "text/html" && responseContent.Length >= 13 && responseContent.Substring(0, 14).ToUpper() == "<!DOCTYPE HTML")
            {
                // Response content is index.html from ProKnow, most likely due to invalid base URL
                var baseUrlWithoutApi = _baseUrl.Substring(0, _baseUrl.Length - 4);
                throw new ProKnowHttpException(method.ToString(), $"{_baseUrl}/{route}", HttpStatusCode.NotFound.ToString(), $"Please verify the base URL '{baseUrlWithoutApi}'.");
            }
            return responseContent;
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
            var response = await MakeRequest(method, route, queryParameters, headerKeyValuePairs, content);
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
            IList <KeyValuePair<string, string>> headerKeyValuePairs = null, HttpContent content = null)
        {
            // Note that the URI ends up with a double slash after api, e.g., 'https://example.proknow.com/api//status'.  Without
            // that double slash, that status route will return 200 OK with a body of "OK", regardless of the credentials provided.
            // With the double slash, that status route will return 401 Unauthorized
            var request = new HttpRequestMessage(method, BuildUriString($"{_baseUrl}/{route}", queryParameters));
            try
            {
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
    }
}
