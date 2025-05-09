using ApiServerWarframe.Hosted;
using ApiServerWarframe.Services.API;
using ApiServerWarframe.Services.API.Clients;
using ApiServerWarframe.Services.API.Interfaces;
using ApiServerWarframe.Services.API.Services;
using ApiServerWarframe.Services.Download;
using ApiServerWarframe.Services.Sorting;
using ApiServerWarframe.Services.State;
using ApiServerWarframe.Services.Storage;

namespace ApiServerWarframe.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWarframeMarketServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpClient>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RestApiClient>>();
                return new RestApiClient("https://api.warframe.market/v1/", logger);
            });

            services.AddSingleton<ItemService>();
            services.AddSingleton<OrderService>();
            services.AddSingleton<StatisticService>();
            services.AddSingleton<Api>();
            return services;
        }

        public static IServiceCollection AddDataProcessingServices(this IServiceCollection services)
        {
            services.AddSingleton<IDataStorage>(new FileDataStorage());
            services.AddSingleton<DataDownloadService>();
            services.AddSingleton<SortingService>();
            services.AddSingleton<DataProcessingState>();
            services.AddHostedService<ScheduledDownloadService>();
            return services;
        }
    }
}
