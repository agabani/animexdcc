using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf.Download
{
    internal class DownloadEpisodeViewModel : BindableBase
    {
        private DownloadProgress _downloadProgress = new DownloadProgress
        {
            PercentageComplete = 40
        };

        private Package _package;

        public Package Package
        {
            get { return _package; }
            set { SetProperty(ref _package, value); }
        }

        public DownloadProgress DownloadProgress
        {
            get { return _downloadProgress; }
            set { SetProperty(ref _downloadProgress, value); }
        }
    }

    internal class DownloadProgress
    {
        public int PercentageComplete { get; set; }
        public string TimeRemaining { get; set; }
        public string TimeElapsed { get; set; }
        public string TransferSpeed { get; set; }
        public string TransferedBytes { get; set; }
        public string RemainingBytes { get; set; }
        public string TotalBytes { get; set; }
    }
}