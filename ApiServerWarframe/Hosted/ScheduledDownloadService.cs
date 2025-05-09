using ApiServerWarframe.Services.Download;
using ApiServerWarframe.Services.Sorting;

namespace ApiServerWarframe.Hosted
{
    public class ScheduledDownloadService : BackgroundService
    {
        private readonly DataDownloadService _downloadService;
        private readonly ILogger<ScheduledDownloadService> _logger;

        public ScheduledDownloadService(DataDownloadService downloadService, ILogger<ScheduledDownloadService> logger)
        {
            _downloadService = downloadService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var startTime = DateTime.Now;

                _logger.LogInformation("Starting data download at: {time}", DateTimeOffset.Now);
                await _downloadService.DownloadDataAsync();
                _logger.LogInformation("Completed data download at: {time}", DateTimeOffset.Now);

                var elapsed = DateTime.Now - startTime;
                var delay = TimeSpan.FromHours(4) - elapsed;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
