using ApiServerWarframe.Extensions;
using ApiServerWarframe.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiServerWarframe.Services.Storage
{
    public class FileDataStorage : IDataStorage
    {
        private const string _folderName = "Data";
        private const string _folderOrderName = "Orders";
        private const string _folderStatisticsName = "Statistics";
        private const string _whiteDirectoryName = "WhiteList";
        private const string _blackDirectoryName = "BlackList";
        private const string _folderItemDetailsName = "ItemDetails";
        private const string _90DaysWhiteListDirectoryName = "90DaysWhiteList";
        private const string _48HoursWhiteListDirectoryName = "48HoursWhiteList";
        private const string _90DaysBlackListDirectoryName = "90DaysBlackList";
        private const string _48HoursBlackListDirectoryName = "48HoursBlackList";

        private static readonly string _pathDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _folderName);
        private static readonly string _pathOrderDirectory = Path.Combine(_pathDirectory, _folderOrderName);
        private static readonly string _pathStatisticsDirectory = Path.Combine(_pathDirectory, _folderStatisticsName);
        private static readonly string _pathWhiteDirectory = Path.Combine(_pathDirectory, _whiteDirectoryName);
        private static readonly string _pathBlackDirectory = Path.Combine(_pathDirectory, _blackDirectoryName);
        private static readonly string _pathItemDetailsDirectory = Path.Combine(_pathDirectory, _folderItemDetailsName);

        private static readonly string _path90DaysWhiteListDirectory = Path.Combine(_pathWhiteDirectory, _90DaysWhiteListDirectoryName);
        private static readonly string _path48HoursWhiteListDirectory = Path.Combine(_pathWhiteDirectory, _48HoursWhiteListDirectoryName);
        private static readonly string _path90DaysBlackListDirectory = Path.Combine(_pathBlackDirectory, _90DaysBlackListDirectoryName);
        private static readonly string _path48HoursBlackListDirectory = Path.Combine(_pathBlackDirectory, _48HoursBlackListDirectoryName);

        private bool AreEqual<T>(T obj1, T obj2)
        {
            if (obj1 == null)
                throw new ArgumentNullException(nameof(obj1));
            if (obj2 == null)
                throw new ArgumentNullException(nameof(obj2));
            var json1 = JToken.FromObject(obj1);
            var json2 = JToken.FromObject(obj2);
            return JToken.DeepEquals(json1, json2);
        }

        public void WriteAllTextSafe(string path, string content)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(path, content);
        }

        public bool HasItems(string language)
        {
            string path = GetItemsFilePath(language);
            return File.Exists(path);
        }

        public void SaveJson<T>(string path, T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            WriteAllTextSafe(path, json);
        }

        public T? LoadJson<T>(string path)
        {
            if (!File.Exists(path))
                return default;

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public void DeleteAllFilesInDirectory(string dir, string searchPattern = "*.json")
        {
            if (!Directory.Exists(dir))
                return;

            var files = Directory.GetFiles(dir, searchPattern, SearchOption.AllDirectories);
            foreach (var file in files)
                File.Delete(file);
        }

        public void SaveItems(IEnumerable<Item> items, string language = "ru")
        {
            string path = GetItemsFilePath(language);
            SaveJson(path, items);
            //FileHelper.WriteAllTextSafe(path, JsonConvert.SerializeObject(items, Formatting.Indented));
        }

        public IEnumerable<Item> GetItems(string language = "ru")
        {
            string path = GetItemsFilePath(language);
            var data = LoadJson<List<Item>>(path);
            return data ?? [];
            //if (File.Exists(path))
            //{

            //    var data = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(path));
            //    return data ?? throw new Exception("Failed to get items");
            //}
            //return new List<Item>(); // Если файла нет, возвращаем пустой список
        }

        public string GetItemsFilePath(string language = "ru")
        {
            string fileName = $"Items_{language}.json"; // Формируем имя файла с кодом языка
            return Path.Combine(_pathDirectory, fileName);
        }

        public void SaveOrders(IEnumerable<Order> orders, string urlItem)
        {
            var orderpath = Path.Combine(_pathOrderDirectory, $"{urlItem}.json");
            SaveJson(orderpath, orders);
            //FileHelper.WriteAllTextSafe(orderpath, JsonConvert.SerializeObject(orders, Formatting.Indented));
        }

        public void DeleteAllOrders() => DeleteAllFilesInDirectory(_pathOrderDirectory);

        //if (!Directory.Exists(_pathOrderDirectory))
        //    return;
        //var files = Directory.GetFiles(_pathOrderDirectory, "*.json");
        //for (int i = 0; i < files.Length; i++)
        //{
        //    if (File.Exists(files[i]))
        //        File.Delete(files[i]);
        //}

        public void SaveItemDetails(ItemDetail itemDetails, string urlName)
        {
            var detailsPath = Path.Combine(_pathItemDetailsDirectory, $"{urlName}.json");
            var existdetails = GetItemDetails(urlName);
            if (existdetails != null && AreEqual(existdetails, itemDetails))
            {
                return;
            }
            SaveJson(detailsPath, itemDetails);
            //FileHelper.WriteAllTextSafe(detailsPath, JsonConvert.SerializeObject(itemDetails, Formatting.Indented));
        }

        public void DeleteAllItemDetails() => DeleteAllFilesInDirectory(_pathItemDetailsDirectory);

        //if (!Directory.Exists(_pathItemDetailsDirectory))
        //    return;
        //var files = Directory.GetFiles(_pathItemDetailsDirectory, "*.json");
        //for (int i = 0; i < files.Length; i++)
        //{
        //    if (File.Exists(files[i]))
        //        File.Delete(files[i]);
        //}

        public ItemDetail? GetItemDetails(string urlName)
        {
            var detailsPath = Path.Combine(_pathItemDetailsDirectory, $"{urlName}.json");
            return LoadJson<ItemDetail>(detailsPath);
            //if (File.Exists(detailsPath))
            //{
            //    return JsonConvert.DeserializeObject<ItemDetail>(File.ReadAllText(detailsPath));
            //}
            //return null;
            //throw new Exception("Item details not found");
        }

        public IEnumerable<ItemDetail> GetItemDetails()
        {
            var files = Directory.GetFiles(_pathItemDetailsDirectory);
            var items = new List<ItemDetail>();
            foreach (var file in files)
            {
                var json = LoadJson<ItemDetail>(file);
                if (json != null)
                    items.Add(json);
            }
            return items;
        }

        public IEnumerable<Order> GetOrders(string urlItem)
        {
            var orderpath = Path.Combine(_pathOrderDirectory, $"{urlItem}.json");
            //if (File.Exists(orderpath))
            //{
            //    var data = JsonConvert.DeserializeObject<List<Order>>(File.ReadAllText(orderpath));
            //    return data ?? throw new Exception("Failed to get orders");
            //}
            var data = LoadJson<List<Order>>(orderpath);
            return data ?? [];
            //return new List<Order>(); // Если файл нет, возвращаем пустой список
        }

        public void SaveStatistics(StatisticsData statistics, string urlItem)
        {
            var statisticsPath = Path.Combine(_pathStatisticsDirectory, $"{urlItem}.json");
            //FileHelper.WriteAllTextSafe(statisticsPath, JsonConvert.SerializeObject(statistics, Formatting.Indented));
            SaveJson(statisticsPath, statistics);
        }

        public void DeleteAllStatistics() => DeleteAllFilesInDirectory(_pathStatisticsDirectory);

        //if (!Directory.Exists(_pathStatisticsDirectory))
        //    return;
        //var files = Directory.GetFiles(_pathStatisticsDirectory, "*.json");
        //for (int i = 0; i < files.Length; i++)
        //{
        //    if (File.Exists(files[i]))
        //        File.Delete(files[i]);
        //}

        public StatisticsData? GetStatistics(string urlItem)
        {
            var statisticsPath = Path.Combine(_pathStatisticsDirectory, $"{urlItem}.json");
            //if (File.Exists(statisticsPath))
            //{
            //    var q = File.ReadAllText(statisticsPath);
            //    var stats = JsonConvert.DeserializeObject<StatisticsData>(q);
            //    return stats;
            //}
            //return null;
            return LoadJson<StatisticsData>(statisticsPath);
        }

        public void AddItemtoWhiteList(SortedItem Item, bool is90Days = true)
        {
            var itempath = Item.UrlName;
            if (Item.Subtype != null)
                itempath += $"_{Item.Subtype}";
            else if (Item.Rank != null)
                itempath += $"_{Item.Rank}";
            var details = GetItemDetails(Item.UrlName);
            Item.TradingTax = details?.ItemsInSet.FirstOrDefault(x => x.UrlName == Item.UrlName)?.TradingTax ?? 0;
            var statistics = GetStatistics(Item.UrlName);
            Item.HoursTrend = statistics?.Last48Hours?.GetWeightedTrend() ?? 0;
            Item.DaysTrend = statistics?.Last90Days?.GetWeightedTrend() ?? 0;
            var path = is90Days ? Path.Combine(_path90DaysWhiteListDirectory, itempath + ".json") : Path.Combine(_path48HoursWhiteListDirectory, itempath + ".json");
            var json = JsonConvert.SerializeObject(Item, Formatting.Indented);
            WriteAllTextSafe(path, json);
        }

        public void AddItemtoBlackList(SortedItem Item, bool is90Days = true)
        {
            var itempath = Item.UrlName;
            if (Item.Subtype != null)
                itempath += $"_{Item.Subtype}";
            else if (Item.Rank != null)
                itempath += $"_{Item.Rank}";
            var path = is90Days ? Path.Combine(_path90DaysBlackListDirectory, itempath + ".json") : Path.Combine(_path48HoursBlackListDirectory, itempath + ".json");
            var json = JsonConvert.SerializeObject(Item, Formatting.Indented);
            WriteAllTextSafe(path, json);
        }

        public void DeleteItemsFromWhiteList() => DeleteAllFilesInDirectory(_pathWhiteDirectory);

        public void DeleteItemsFromBlackList() => DeleteAllFilesInDirectory(_pathBlackDirectory);

        //public SortedItem? DeleteItemFromWhiteList(string urlItem, string type)
        //{
        //    var path = Path.Combine(_pathWhiteDirectory, urlItem + $"_{type}" + ".json");
        //    var fileExist = File.Exists(path);
        //    if (fileExist)
        //    {
        //        var item = JsonConvert.DeserializeObject<SortedItem>(File.ReadAllText(path));
        //        return item;
        //    }
        //    return null;
        //}

        //public SortedItem? DeleteItemFromBlackList(string urlItem, string type)
        //{
        //    var path = Path.Combine(_pathBlackDirectory, urlItem + $"_{type}" + ".json");
        //    var fileExist = File.Exists(path);
        //    if (fileExist)
        //    {
        //        var item = JsonConvert.DeserializeObject<SortedItem>(File.ReadAllText(path));
        //        return item;
        //    }
        //    return null;
        //}

        public IEnumerable<SortedItem> GetWhiteList(bool is48Hours = true, string language = "ru") //TODO: доделать поддержку выбора языка
        {
            var dirPath = is48Hours ? _path48HoursWhiteListDirectory : _path90DaysWhiteListDirectory;
            if (!Directory.Exists(dirPath))
            {
                return [];
            }

            var files = Directory.GetFiles(dirPath, "*.json");
            var items = new List<SortedItem>();

            for (int i = 0; i < files.Length; i++)
            {
                //var item = JsonConvert.DeserializeObject<SortedItem>(File.ReadAllText(files[i])); //TODO: пофиксить проблему доступа к файлу
                var item = LoadJson<SortedItem>(files[i]);

                if (item == null)
                    continue;

                var details = GetItems(language);
                item.Name = details.FirstOrDefault(x => x.UrlName == item.UrlName)?.ItemName ?? item.Name;
                items.Add(item);
            }

            return items;
        }

        public IEnumerable<SortedItem> GetBlackList(bool is48Hours = true, string language = "ru") //TODO: доделать поддержку выбора языка
        {
            var dirPath = is48Hours ? _path48HoursBlackListDirectory : _path90DaysBlackListDirectory;
            if (!Directory.Exists(dirPath))
            {
                return [];
            }

            var files = Directory.GetFiles(dirPath, "*.json");
            var items = new List<SortedItem>();

            for (int i = 0; i < files.Length; i++)
            {
                var item = LoadJson<SortedItem>(files[i]);

                if (item == null)
                    continue;

                var details = GetItems(language);
                item.Name = details.FirstOrDefault(x => x.UrlName == item.UrlName)?.ItemName ?? item.Name;
                items.Add(item);
            }
            return items;
        }

        private DateTime GetLatestFileWriteTime(string folderPath)
        {
            var directoryInfo = new DirectoryInfo(folderPath);
            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException($"Папка не найдена: {folderPath}");

            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            DateTime latest = directoryInfo.LastWriteTime;

            foreach (var file in files)
            {
                if (file.LastWriteTime > latest)
                    latest = file.LastWriteTime;
            }

            return latest;
        }

        public DateTime GetWhiteListUpdateTime() => GetLatestFileWriteTime(_pathWhiteDirectory);

        public bool FileExists(string path) => File.Exists(path);
    }
}
