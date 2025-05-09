namespace ApiServerWarframe.Services.API.Interfaces
{
    public interface IHttpClient
    {
        Task<T?> GetAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null);
        Task<T?> PostAsync<T>(string endpoint, object? body = null, Dictionary<string, string>? headers = null);
        Task<T?> PutAsync<T>(string endpoint, object body, Dictionary<string, string>? headers = null);
        Task<bool> DeleteAsync(string endpoint, object? body = null, Dictionary<string, string>? headers = null);
    }
}
