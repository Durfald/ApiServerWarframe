namespace ApiServerWarframe.Services.State
{
    public class DataProcessingState
    {
        private readonly object _lock = new();
        private bool _isDownloading;
        private bool _isSorting;

        public bool IsDownloading
        {
            get { lock (_lock) { return _isDownloading; } }
            set { lock (_lock) { _isDownloading = value; } }
        }

        public bool IsSorting
        {
            get { lock (_lock) { return _isSorting; } }
            set { lock (_lock) { _isSorting = value; } }
        }

        public DateTime? LastDownloadTime { get; set; }
        public DateTime? LastSortTime { get; set; }
    }
}
