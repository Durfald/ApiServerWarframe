using ApiServerWarframe.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiServerWarframe.Services.Storage
{
    public class DatabaseDataStorage : IDataStorage
    {
        private readonly DbContext _context;

        public DatabaseDataStorage(DbContext context)
        {
            _context = context;
        }

        public void AddItemtoBlackList(SortedItem item, bool is90Days = true)
        {
            throw new NotImplementedException();
        }

        public void AddItemtoWhiteList(SortedItem item, bool is90Days = true)
        {
            throw new NotImplementedException();
        }

        public void DeleteAllItemDetails()
        {
            throw new NotImplementedException();
        }

        public void DeleteAllOrders()
        {
            throw new NotImplementedException();
        }

        public void DeleteAllStatistics()
        {
            throw new NotImplementedException();
        }

        public void DeleteItemsFromBlackList()
        {
            throw new NotImplementedException();
        }

        public void DeleteItemsFromWhiteList()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SortedItem> GetBlackList(bool is48Hours = true, string language = "ru")
        {
            throw new NotImplementedException();
        }

        public ItemDetail? GetItemDetails(string urlName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ItemDetail> GetItemDetails()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Item> GetItems(string language = "ru")
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Order> GetOrders(string urlItem)
        {
            throw new NotImplementedException();
        }

        public StatisticsData? GetStatistics(string urlItem)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SortedItem> GetWhiteList(bool is48Hours = true, string language = "ru")
        {
            throw new NotImplementedException();
        }

        public DateTime GetWhiteListUpdateTime()
        {
            throw new NotImplementedException();
        }

        public bool HasItems(string Language = "ru")
        {
            throw new NotImplementedException();
        }

        public void SaveItemDetails(ItemDetail itemDetails, string urlName)
        {
            throw new NotImplementedException();
        }

        public void SaveItems(IEnumerable<Item> items, string language = "ru")
        {
            throw new NotImplementedException();
        }

        public void SaveOrders(IEnumerable<Order> orders, string urlItem)
        {
            throw new NotImplementedException();
        }

        public void SaveStatistics(StatisticsData statistics, string urlItem)
        {
            throw new NotImplementedException();
        }
    }

}
