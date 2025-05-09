using ApiServerWarframe.Models;
using ApiServerWarframe.Services.API.Interfaces;

namespace ApiServerWarframe.Services.API.Services
{
    public class OrderService
    {
        private readonly IHttpClient _apiClient;

        public OrderService(IHttpClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Order>> GetOrdersAsync(string urlName, string platform = "pc")
        {
            var headers = new Dictionary<string, string> { { "Platform", platform } };
            var response = await _apiClient.GetAsync<ApiResponse<OrdersPayload>>($"items/{urlName}/orders", headers: headers);

            return response?.Payload?.Orders ?? throw new Exception("Orders not found");
        }
    }
}
