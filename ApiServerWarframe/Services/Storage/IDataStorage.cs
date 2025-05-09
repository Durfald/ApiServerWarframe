using ApiServerWarframe.Models;

namespace ApiServerWarframe.Services.Storage
{
    public interface IDataStorage
    {
        bool HasItems(string Language = "ru");
        void SaveItems(IEnumerable<Item> items, string language = "ru");
        IEnumerable<Item> GetItems(string language = "ru");

        void SaveOrders(IEnumerable<Order> orders, string urlItem);
        IEnumerable<Order> GetOrders(string urlItem);

        void SaveItemDetails(ItemDetail itemDetails, string urlName);
        ItemDetail? GetItemDetails(string urlName);
        IEnumerable<ItemDetail> GetItemDetails();

        void SaveStatistics(StatisticsData statistics, string urlItem);
        StatisticsData? GetStatistics(string urlItem);

        //void AddItemtoWhiteList(SortedItem item, bool is90Days = true);
        //void AddItemtoBlackList(SortedItem item, bool is90Days = true);
        //IEnumerable<SortedItem> GetWhiteList(bool is48Hours = true, string language = "ru");
        //IEnumerable<SortedItem> GetBlackList(bool is48Hours = true, string language = "ru");
        //DateTime GetWhiteListUpdateTime();

        void DeleteAllOrders();
        void DeleteAllItemDetails();
        void DeleteAllStatistics();
        //void DeleteItemsFromWhiteList();
        //void DeleteItemsFromBlackList();
    }

}
