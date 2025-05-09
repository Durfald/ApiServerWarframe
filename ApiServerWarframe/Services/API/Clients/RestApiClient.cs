using ApiServerWarframe.Services.API.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace ApiServerWarframe.Services.API.Clients
{
    public class RestApiClient : IHttpClient
    {
        private readonly RestClient _client;
        private readonly ILogger<RestApiClient> _logger;

        //public readonly string Version;


        public RestApiClient(string baseUrl, ILogger<RestApiClient> logger)
        {
            _client = new RestClient(baseUrl);
            _logger = logger;
        }

        private async Task<T?> ExecuteRequest<T>(RestRequest request)
        {
            var response = await _client.ExecuteAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning($"API request failed\r\n. StatusCode: {response.StatusCode}, Path: {request.Resource}, Content: {response.Content}");
                return default;
            }
            if (response.Content == null)
            {
                _logger.LogWarning($"API response is null\r\n. StatusCode: {response.StatusCode}, Path: {request.Resource}, Content: {response.Content}");
                return default;
            }
            var data = JsonConvert.DeserializeObject<T>(response.Content);
            if (data == null)
            {
                _logger.LogWarning($"API deserialize failed\r\n. StatusCode: {response.StatusCode}, Path: {request.Resource}, Content: {response.Content}");
                return default;
            }

            return data;
        }

        public async Task<T?> GetAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(endpoint, Method.Get);

            if (body != null)
                request.AddJsonBody(body);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);
            }

            return await ExecuteRequest<T>(request);
        }

        public async Task<T?> PostAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(endpoint, Method.Post);

            if (body != null)
                request.AddJsonBody(body);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);
            }

            return await ExecuteRequest<T>(request);
        }

        public async Task<T?> PutAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(endpoint, Method.Put);

            if (body != null)
                request.AddJsonBody(body);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);
            }

            return await ExecuteRequest<T>(request);
        }

        public async Task<bool> DeleteAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null)
        {
            var request = new RestRequest(endpoint, Method.Delete);

            if (body != null)
                request.AddJsonBody(body);

            if (headers != null)
            {
                foreach (var header in headers)
                    request.AddHeader(header.Key, header.Value);
            }
            var response = await _client.ExecuteAsync(request);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }

}
