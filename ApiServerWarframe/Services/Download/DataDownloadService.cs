using ApiServerWarframe.Models;
using ApiServerWarframe.Services.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using ApiServerWarframe.Services.State;
using ApiServerWarframe.Services.Storage;

namespace ApiServerWarframe.Services.Download
{
    public class DataDownloadService
    {
        private readonly Api _api;
        private readonly DataProcessingState _state;
        private readonly IDataStorage _fileHelper;

        private async Task DownloadItemDetails(string urlName)
        {
            var itemDetails = await _api.GetItemDetailsAsync(urlName);
            _fileHelper.SaveItemDetails(itemDetails, urlName);
        }

        private async Task DownloadOrders(string urlName)
        {
            var orders = await _api.GetOrdersAsync(urlName);
            _fileHelper.SaveOrders(orders, urlName);
        }

        private async Task DownloadStatistics(string urlName)
        {
            var statistics = await _api.GetStatisticsAsync(urlName);
            _fileHelper.SaveStatistics(statistics, urlName);
        }

        public DataDownloadService(Api api, DataProcessingState state, IDataStorage fileHelper)
        {
            _api = api;
            _state = state;
            _fileHelper = fileHelper;
        }

        public async Task DownloadLanguageAsync(string language)
        {
            var itemsExist = _fileHelper.HasItems(language);
            if (!itemsExist)
            {
                var items = await _api.GetItemsAsync(language);
                _fileHelper.SaveItems(items, language);
            }
        }

        private async Task DownloadAllLanguages()
        {
            var languages = new[] { "ru", "en", "de", "fr", "ko", "zh-hant", "zh-hans", "pt", "es", "it", "pl", "uk" };
            foreach (var language in languages)
            {
                await DownloadLanguageAsync(language);
            }
        }

        public async Task DownloadDataAsync(string Language = "ru")
        {
            await DownloadAllLanguages();

            _state.IsDownloading = true;
            try
            {
                var itemsExist = _fileHelper.HasItems(Language);
                IEnumerable<Item> items;

                if (!itemsExist)
                {
                    await DownloadLanguageAsync(Language);
                }

                items = _fileHelper.GetItems(Language);

                //var items = File.Exists(path) ? _fileHelper.GetItems(Language) : await _api.GetItemsAsync(Language);


                _fileHelper.DeleteAllOrders();
                _fileHelper.DeleteAllStatistics();
                var totalItems = items.Count();
                int index = 0;
                int maxParallelRequests = 1; // Ограничение: максимум 2 предмета одновременно
                var semaphore = new SemaphoreSlim(maxParallelRequests);

                var tasks = items.Select(async item =>
                {
                    await semaphore.WaitAsync(); // Ждем свободный слот
                    try
                    {
                        int itemIndex = Interlocked.Increment(ref index);
                        //Debug.WriteLine($"Starting {item.ItemName} | {itemIndex} / {totalItems}");

                        // Разделяем загрузку ордеров и статистики
                        var downloadOrdersTask = DownloadOrders(item.UrlName);
                        var downloadStatisticsTask = DownloadStatistics(item.UrlName);
                        var downloadItemDetailsTask = DownloadItemDetails(item.UrlName);
                        await Task.WhenAll(downloadOrdersTask, downloadStatisticsTask, downloadItemDetailsTask); // Ожидаем завершения всех задач

                        await Task.Delay(1000); // Искусственная задержка для API
                    }
                    finally
                    {
                        semaphore.Release(); // Освобождаем слот
                    }

                }).ToList();

                await Task.WhenAll(tasks); // Ждем завершения всех задач
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //TODO: Придумать логирование
            }
            finally
            {
                _state.IsDownloading = false;
            }
        }
    }
}
