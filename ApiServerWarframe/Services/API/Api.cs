using ApiServerWarframe.Models;
using ApiServerWarframe.Services.API.Services;

namespace ApiServerWarframe.Services.API
{
    public class Api
    {
        private readonly ItemService _itemService;
        private readonly OrderService _orderService;
        private readonly StatisticService _statisticService;

        public Api(ItemService itemService, OrderService orderService, StatisticService statisticService)
        {
            _itemService = itemService;
            _orderService = orderService;
            _statisticService = statisticService;
        }

        public Task<List<Item>> GetItemsAsync(string language = "ru") => _itemService.GetItemsAsync(language);
        public Task<ItemDetail> GetItemDetailsAsync(string urlItem, string platform = "pc") => _itemService.GetItemDetailsAsync(urlItem, platform);
        public Task<List<Order>> GetOrdersAsync(string urlItem, string platform = "pc") => _orderService.GetOrdersAsync(urlItem, platform);
        public Task<StatisticsData> GetStatisticsAsync(string urlItem, string platform = "pc") => _statisticService.GetStatisticsAsync(urlItem, platform);
    }
}