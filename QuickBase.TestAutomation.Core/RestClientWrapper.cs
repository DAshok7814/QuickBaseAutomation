using Microsoft.Extensions.Configuration;
using RestSharp;
using Serilog;

public class RestClientWrapper
{
    private readonly RestClient _client;
    private readonly int _maxRetries;
    private readonly int _initialDelayMs;
    private readonly int _timeOut;


    public RestClientWrapper(IConfigurationRoot _configuration)
    {
        if (_configuration == null)
        {
            Log.Error("Configuration is not initialized.");
            throw new InvalidOperationException("Configuration is not initialized.");
        }

        var baseUrl = _configuration["apiSettings:baseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            Log.Error("Base URL is not configured.");
            throw new InvalidOperationException("Base URL is not configured.");
        }

        _client = new RestClient(baseUrl);
        _maxRetries = int.Parse(_configuration?["apiSettings:maxRetryAttempts"]);
        _initialDelayMs = int.Parse(_configuration["apiSettings:pauseBetweenFailures"]);
        _timeOut = int.Parse(_configuration["apiSettings:clientTimeoutSeconds"]);
    }

    /// <summary>
    /// Handles the request with retry logic
    /// </summary>
    private async Task<RestResponse> ExecuteWithRetryAsync(RestRequest request)
    {
        int retryCount = 0;
        int delay = _initialDelayMs;
        request.Timeout = new TimeSpan(0, 0, 0, _timeOut, 0); // 30 seconds
        while (retryCount < _maxRetries)
        {
            var response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful || (int)response.StatusCode < 500)
                return response; // Success or client error (no retry needed)

            Console.WriteLine($"Request failed ({response.StatusCode}). Retrying in {delay}ms...");
            await Task.Delay(delay);
            delay *= 2; // Exponential backoff
            retryCount++;
        }

        throw new Exception("Max retry attempts reached.");
    }

    /// <summary>
    /// Builds an IRestRequest with headers and optional body.
    /// </summary>
    private RestRequest BuildRequest(string resource, Method method, object body = null, Dictionary<string, string> headers = null)
    {
        var request = new RestRequest(resource, method);

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
        }

        if (body != null)
        {
            request.AddJsonBody(body);
        }

        return request;
    }

    /// <summary>
    /// Sends a GET request.
    /// </summary>
    public async Task<RestResponse> GetAsync(string resource, Dictionary<string, string> headers = null)
    {
        var request = BuildRequest(resource, Method.Get, null, headers);
        return await ExecuteWithRetryAsync(request);
        
    }

    /// <summary>
    /// Sends a POST request.
    /// </summary>
    public async Task<RestResponse> PostAsync<T>(string resource, T body, Dictionary<string, string> headers = null)
    {
        var request = BuildRequest(resource, Method.Post, body, headers);
        return  await ExecuteWithRetryAsync(request);
    }

    /// <summary>
    /// Sends a PUT request.
    /// </summary>
    public async Task<RestResponse> PutAsync<T>(string resource, T body, Dictionary<string, string> headers = null)
    {
        var request = BuildRequest(resource, Method.Put, body, headers);
        return await ExecuteWithRetryAsync(request);
    }

    /// <summary>
    /// Sends a DELETE request.
    /// </summary>
    public async Task<RestResponse> DeleteAsync(string resource, Dictionary<string, string> headers = null)
    {
        var request = BuildRequest(resource, Method.Delete, null, headers);
        return await ExecuteWithRetryAsync(request);
    }
}