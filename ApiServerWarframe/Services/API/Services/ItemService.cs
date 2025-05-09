using ApiServerWarframe.Models;
using ApiServerWarframe.Services.API.Interfaces;

namespace ApiServerWarframe.Services.API.Services
{
    public class ItemService
    {
        private readonly IHttpClient _apiClient;

        public ItemService(IHttpClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<Item>> GetItemsAsync(string language = "ru")
        {
            var headers = new Dictionary<string, string> { { "Language", language } };
            var response = await _apiClient.GetAsync<ApiResponse<ItemsPayload>>("items", headers: headers);

            return response?.Payload?.Items ?? throw new Exception("Failed to get items");
        }

        public async Task<ItemDetail> GetItemDetailsAsync(string urlItem, string platform = "pc")
        {
            var headers = new Dictionary<string, string> { { "Platform", platform } };
            var response = await _apiClient.GetAsync<ApiResponse<ItemDetailPayload>>($"items/{urlItem}", headers: headers);
            return response?.Payload?.Item ?? throw new Exception("Failed to get item details");
        }
    }

}
