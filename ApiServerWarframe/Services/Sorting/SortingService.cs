using ApiServerWarframe.Models;
using ApiServerWarframe.Services.API;
using ApiServerWarframe.Services.State;
using ApiServerWarframe.Services.Storage;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace ApiServerWarframe.Services.Sorting
{
    public class SortingService
    {
        private readonly DataProcessingState _state;
        private readonly IDataStorage _fileHelper;
        private readonly ILogger<SortingService> _logger;

        public SortingService(DataProcessingState state, IDataStorage fileHelper, ILogger<SortingService> logger)
        {
            _state = state;
            _fileHelper = fileHelper;
            _logger = logger;
        }

        public async Task<List<SortedItem>> SortData(string language = "ru", int minSpread = 15, int minLiquidity = 60)
        {
            List<SortedItem> sortedItems = new List<SortedItem>();
            _state.IsSorting = true;
            var itemsExist = _fileHelper.HasItems(language);
            IEnumerable<Item> items = new List<Item>();
            if (itemsExist)
            {
                items = _fileHelper.GetItems(language);
            }
            else
            {
                _logger.LogWarning("Failed to get items");
                return sortedItems;
            }

            int totalItems = items.Count();
            int maxParallelTasks = Environment.ProcessorCount;
            var semaphore = new SemaphoreSlim(maxParallelTasks);

            var tasks = new List<Task>();

            foreach (var item in items)
            {
                await semaphore.WaitAsync(); // Ограничиваем количество параллельных задач
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        var statistic = _fileHelper.GetStatistics(item.UrlName);
                        var orders = _fileHelper.GetOrders(item.UrlName);
                        if (statistic == null)
                        {
                            _logger.LogWarning($"Statistics not found for item {item.ItemName}. urlItem: {item.UrlName}");
                            return;
                            //DownloadStatistics(item.UrlName);
                            //await Task.Delay(1000);
                            //statistic = _fileHelper.GetStatistics(item.UrlName);
                            //Debug.WriteLine($"Нету статистики {item.ItemName}");
                        }
                        if (orders == null)
                        {
                            _logger.LogWarning($"Orders not found for item {item.ItemName}. urlItem: {item.UrlName}");
                            return;
                            //await DownloadOrders(item.UrlName);
                            //await Task.Delay(1000);
                            //orders = GetOrders(item.UrlName);
                            //Debug.WriteLine($"Нету ордеров {item.ItemName}");
                        }
                        var itemDetail = _fileHelper.GetItemDetails(item.UrlName);
                        if (itemDetail == null)
                        {
                            _logger.LogWarning($"Item Detail not found for item {item.ItemName}. urlItem: {item.UrlName}");
                            return;
                            //await DownloadItemDetails(item.UrlName);
                            //itemDetail = GetItemDetails(item.UrlName);
                            //Debug.WriteLine($"Нету деталей {item.ItemName}");
                        }

                        var details = itemDetail.ItemsInSet.FirstOrDefault(x => x.UrlName == item.UrlName);
                        if (details == null)
                        {
                            _logger.LogWarning($"Item Detail not found for item {item.ItemName}. urlItem: {item.UrlName}");
                            return;
                        }

                        var _existRank = details.ModMaxRank != null;
                        var _existSubtype = details.Subtypes != null;
                        var twoDaysAgo = DateTime.Now.Date.AddDays(-2);
                        var yesterday = DateTime.Now.Date.AddDays(-1);

                        #region sorting_90_days
                        var lastTwoDaysStats = statistic.Last90Days
                            .Where(x => x.Datetime.Date == twoDaysAgo.Date || x.Datetime.Date == yesterday.Date)
                            .ToList();

                        if (_existSubtype)
                        {
                            var subtypes = details.Subtypes;
                            if (subtypes == null)
                            {
                                _logger.LogWarning($"Subtypes not found for item {item.ItemName}. urlItem: {item.UrlName}");
                                return;
                            }
                            ProcessSubtupeStatistics(item, orders, minSpread, minLiquidity, lastTwoDaysStats, subtypes, ref sortedItems);
                        }
                        else if (_existRank)
                        {
                            if (details.ModMaxRank == null)
                            {
                                throw new Exception($"Subtypes not found for item {item.ItemName}. urlItem: {item.UrlName}");
                                return;
                            }

                            var maxRank = details.ModMaxRank;

                            ProcessRankedStatistics(item, orders, minSpread, minLiquidity, lastTwoDaysStats, maxRank.Value, ref sortedItems);
                        }
                        else
                        {
                            ProcessUnrankedStatistics(item, orders, minSpread, minLiquidity, lastTwoDaysStats, ref sortedItems);
                        }
                        #endregion

                        #region sorting_48_hours
                        if (_existSubtype)
                        {
                            var subtypes = details.Subtypes;
                            if (subtypes == null)
                            {
                                _logger.LogWarning($"Subtypes not found for item {item.ItemName}. urlItem: {item.UrlName}");
                                return;
                            }
                            ProcessSubtupeStatistics(item, orders, minSpread, minLiquidity, lastTwoDaysStats, subtypes, ref sortedItems);
                        }
                        else if (_existRank)
                        {
                            if (details.ModMaxRank == null)
                            {
                                throw new Exception($"Subtypes not found for item {item.ItemName}. urlItem: {item.UrlName}");
                                return;
                            }
                            var maxRank = details.ModMaxRank;
                            ProcessRankedStatistics(item, orders, minSpread, minLiquidity, lastTwoDaysStats, maxRank.Value, ref sortedItems);
                        }
                        else
                        {
                            ProcessUnrankedStatistics(item, orders, minSpread, minLiquidity, statistic.Last48Hours, ref sortedItems, false);
                        }
                        #endregion
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks); // Ждем завершения всех задач
            _state.IsSorting = false;
            return sortedItems;
        }

        #region ProcessStatistics
        private void ProcessRankedStatistics(
            Item item,
            IEnumerable<Order> orders,
            int minSpread,
            int minLiquidity,
            IEnumerable<Statistic> lastTwoDaysStats,
            int MaxRank,
            ref List<SortedItem> sortedItems,
            bool is90Days = true)
        {
            var maxRStats = lastTwoDaysStats.Where(x => x.ModRank > 0).ToList();
            var minRStats = lastTwoDaysStats.Where(x => x.ModRank == 0).ToList();

            var maxRvolumesum = maxRStats.Sum(x => x.Volume);
            var minRvolumesum = minRStats.Sum(x => x.Volume);

            // Debug.WriteLine($"Объем maxR: {maxRvolumesum} | Объем minR: {minRvolumesum} | {item.ItemName}");

            bool skipMaxR = maxRvolumesum <= minLiquidity;
            bool skipMinR = minRvolumesum <= minLiquidity;

            if (skipMaxR || MaxRank == 99)
            {
                //if (MaxRank != 99)
                //    _fileHelper.AddItemtoBlackList(new SortedItem
                //    {
                //        UrlName = item.UrlName,
                //        Id = item.Id,
                //        Rank = MaxRank,
                //        Liquidity = maxRvolumesum,
                //        Reason = "Недостаточно информации",
                //    }, is90Days);
                //else
                //    _fileHelper.AddItemtoBlackList(new SortedItem
                //    {
                //        UrlName = item.UrlName,
                //        Id = item.Id,
                //        Rank = MaxRank,
                //        Liquidity = maxRvolumesum,
                //        Reason = "Минимальная ликвидность",
                //    }, is90Days);
                return;
            }

            if (skipMinR)
            {
                //_fileHelper.AddItemtoBlackList(new SortedItem
                //{
                //    UrlName = item.UrlName,
                //    Id = item.Id,
                //    Rank = 0,
                //    Liquidity = minRvolumesum,
                //    Reason = "Минимальная ликвидность",
                //}, is90Days);
                return;
            }

            if (!skipMaxR) ProcessRankedOrders(item, orders, minSpread, true, ref sortedItems, is90Days);
            if (!skipMinR) ProcessRankedOrders(item, orders, minSpread, false, ref sortedItems, is90Days);
        }

        private void ProcessUnrankedStatistics(
            Item item,
            IEnumerable<Order> orders,
            int minSpread,
            int minLiquidity,
            IEnumerable<Statistic> lastTwoDaysStats,
            ref List<SortedItem> sortedItems,
            bool is90Days = true)
        {
            var volumesum = lastTwoDaysStats.Sum(x => x.Volume);
            bool skip = volumesum <= minLiquidity;

            if (skip)
            {
                //_fileHelper.AddItemtoBlackList(new SortedItem
                //{
                //    UrlName = item.UrlName,
                //    Id = item.Id,
                //    Liquidity = volumesum,
                //    Reason = "Минимальная ликвидность",
                //}, is90Days);
                return;
            }
            if (!skip) ProcessUnrankedOrders(item, orders, minSpread, ref sortedItems, is90Days);
        }

        private void ProcessSubtupeStatistics(
            Item item,
            IEnumerable<Order> orders,
            int minSpread,
            int minLiquidity,
            IEnumerable<Statistic> lastTwoDaysStats,
            IEnumerable<string> uniqueSubtypes,
            ref List<SortedItem> sortedItems,
            bool is90Days = true)
        {
            if (uniqueSubtypes.Count() == 0)
            {
                //_fileHelper.AddItemtoBlackList(new SortedItem
                //{
                //    UrlName = item.UrlName,
                //    Id = item.Id,
                //    Reason = "Недостаточно информации"
                //}, is90Days);
                return;
            }

            foreach (var subtype in uniqueSubtypes)
            {
                var lastTwoDaysSubtypeStats = lastTwoDaysStats.Where(x => x.Subtype == subtype).ToList();
                var volumeSum = lastTwoDaysSubtypeStats.Sum(x => x.Volume);

                //Debug.WriteLine($"Объем продаж для подтипа {subtype}: {volumeSum} | {item.ItemName}");

                if (volumeSum <= minLiquidity)
                {
                    //_fileHelper.AddItemtoBlackList(new SortedItem
                    //{
                    //    UrlName = item.UrlName,
                    //    Id = item.Id,
                    //    Liquidity = volumeSum,
                    //    Subtype = subtype,
                    //    Reason = "Минимальная ликвидность для подтипа"
                    //}, is90Days);
                }
                else
                {
                    ProcessOrdersWithSubtype(item, orders, minSpread, subtype, ref sortedItems, is90Days);
                }
            }
        }
        #endregion

        #region ProcessOrders
        private void ProcessOrdersWithSubtype(
            Item item,
            IEnumerable<Order> orders,
            int minSpread,
            string subtype,
            ref List<SortedItem> sortedItems,
            bool is90Days = true)
        {
            var buyOrder = orders
                .Where(
                x => x.OrderTypeEnum == OrderType.Buy &&
                x.Subtype == subtype &&
                x.Visible == true &&
                x.User.StatusEnum == UserStatus.InGame)
                .OrderByDescending(x => x.Platinum).FirstOrDefault();

            var sellOrder = orders
                .Where(
                x => x.OrderTypeEnum == OrderType.Sell &&
                x.Subtype == subtype &&
                x.Visible == true &&
                x.User.StatusEnum == UserStatus.InGame)
                .OrderBy(x => x.Platinum).FirstOrDefault();

            if (buyOrder == null || sellOrder == null)
            {
                //var sorteditem = new SortedItem();
                //sorteditem.Id = item.Id;
                //sorteditem.UrlName = item.UrlName;
                //sorteditem.Subtype = subtype;
                //sorteditem.Reason = "Нет заказа";

                //_fileHelper.AddItemtoBlackList(sorteditem, is90Days);
                return;
            }

            if (sellOrder.Platinum - buyOrder.Platinum < minSpread)
            {
                //var sorteditem = new SortedItem();
                //sorteditem.Id = item.Id;
                //sorteditem.UrlName = item.UrlName;
                //sorteditem.Subtype = subtype;
                //sorteditem.Reason = "Минимальная разница";
                //sorteditem.BuyPrice = buyOrder.Platinum;
                //sorteditem.SellPrice = sellOrder.Platinum;
                //sorteditem.Spread = sellOrder.Platinum - buyOrder.Platinum;

                //_fileHelper.AddItemtoBlackList(sorteditem, is90Days);
                return;
            }
            else
            {
                var sorteditem = new SortedItem();
                sorteditem.Id = item.Id;
                sorteditem.UrlName = item.UrlName;
                sorteditem.Subtype = subtype;
                sorteditem.SellPrice = sellOrder.Platinum;
                sorteditem.BuyPrice = buyOrder.Platinum;
                sorteditem.Spread = sellOrder.Platinum - buyOrder.Platinum;
                sortedItems.Add(sorteditem);
                //_fileHelper.AddItemtoWhiteList(sorteditem, is90Days);
            }
        }

        // Обработка заказов с рангом
        private void ProcessRankedOrders(
            Item item,
            IEnumerable<Order> orders,
            int minSpread,
            bool isMaxRank,
            ref List<SortedItem> sortedItems,
            bool is90Days = true)
        {
            var rankCondition = isMaxRank ? orders?.FirstOrDefault(x => x.ModRank > 0)?.ModRank : 0;
            var buyOrder = orders
                .Where(
                x => x.OrderTypeEnum == OrderType.Buy &&
                x.ModRank == rankCondition &&
                x.Visible == true &&
                x.User.StatusEnum == UserStatus.InGame)
                .OrderByDescending(x => x.Platinum).FirstOrDefault();

            var sellOrder = orders
                .Where(
                x => x.OrderTypeEnum == OrderType.Sell &&
                x.ModRank == rankCondition &&
                x.Visible == true &&
                x.User.StatusEnum == UserStatus.InGame)
                .OrderBy(x => x.Platinum).FirstOrDefault();

            if (buyOrder == null || sellOrder == null)
            {
                //var sorteditem = new SortedItem();
                //sorteditem.Id = item.Id;
                //sorteditem.UrlName = item.UrlName;
                //sorteditem.Rank = rankCondition;
                //sorteditem.Reason = "Нет заказа";

                //_fileHelper.AddItemtoBlackList(sorteditem, is90Days);
                return;
            }

            if (sellOrder.Platinum - buyOrder.Platinum <= minSpread)
            {
                //var sorteditem = new SortedItem();
                //sorteditem.Id = item.Id;
                //sorteditem.UrlName = item.UrlName;
                //sorteditem.Rank = rankCondition;
                //sorteditem.Reason = "Минимальная разница";
                //sorteditem.BuyPrice = buyOrder.Platinum;
                //sorteditem.SellPrice = sellOrder.Platinum;
                //sorteditem.Spread = sellOrder.Platinum - buyOrder.Platinum;

                //_fileHelper.AddItemtoBlackList(sorteditem, is90Days);
                return;
            }
            else
            {
                var sorteditem = new SortedItem();
                sorteditem.Id = item.Id;
                sorteditem.UrlName = item.UrlName;
                sorteditem.Rank = rankCondition;
                sorteditem.SellPrice = sellOrder.Platinum;
                sorteditem.BuyPrice = buyOrder.Platinum;
                sorteditem.Spread = sellOrder.Platinum - buyOrder.Platinum;

                //_fileHelper.AddItemtoWhiteList(sorteditem, is90Days);
                sortedItems.Add(sorteditem);
            }
        }

        // Обработка заказов без ранга
        private void ProcessUnrankedOrders(
            Item item,
            IEnumerable<Order> orders,
            int minSpread,
            ref List<SortedItem> sortedItems,
            bool is90Days = true)
        {
            var buyOrder = orders
                .Where(
                x => x.OrderTypeEnum == OrderType.Buy &&
                x.Visible == true &&
                x.User.StatusEnum == UserStatus.InGame)
                .OrderByDescending(x => x.Platinum).FirstOrDefault();

            var sellOrder = orders
                .Where(
                x => x.OrderTypeEnum == OrderType.Sell &&
                x.Visible == true &&
                x.User.StatusEnum == UserStatus.InGame)
                .OrderBy(x => x.Platinum).FirstOrDefault();

            if (buyOrder == null || sellOrder == null)
            {
                //var sorteditem = new SortedItem();
                //sorteditem.Id = item.Id;
                //sorteditem.UrlName = item.UrlName;
                //sorteditem.Reason = "Нет заказа";

                return;
            }
            if (sellOrder.Platinum - buyOrder.Platinum <= minSpread)
            {
                //var sorteditem = new SortedItem();
                //sorteditem.Id = item.Id;
                //sorteditem.UrlName = item.UrlName;
                //sorteditem.SellPrice = sellOrder.Platinum;
                //sorteditem.BuyPrice = buyOrder.Platinum;
                //sorteditem.Spread = sellOrder.Platinum - buyOrder.Platinum;
                //sorteditem.Reason = "Минимальная разница";
                return;
            }
            else
            {
                var sorteditem = new SortedItem();
                sorteditem.Id = item.Id;
                sorteditem.UrlName = item.UrlName;
                sorteditem.SellPrice = sellOrder.Platinum;
                sorteditem.BuyPrice = buyOrder.Platinum;
                sorteditem.Spread = sellOrder.Platinum - buyOrder.Platinum;
                sortedItems.Add(sorteditem);
            }
        }
        #endregion
    }
}
