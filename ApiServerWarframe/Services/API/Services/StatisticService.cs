using ApiServerWarframe.Models;
using ApiServerWarframe.Services.API.Interfaces;

namespace ApiServerWarframe.Services.API.Services
{
    public class StatisticService
    {
        private readonly IHttpClient _apiClient;

        public StatisticService(IHttpClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<StatisticsData> GetStatisticsAsync(string url_name, string platform = "pc")
        {
            var headers = new Dictionary<string, string> { { "Platform", platform }, { "Crossplay", "true" } };
            var response = await _apiClient.GetAsync<ApiResponse<StatisticsPayload>>($"items/{url_name}/statistics", headers: headers);

            return response?.Payload?.ClosedStatistics ?? throw new Exception("Statistics not found");
        }
    }
}
