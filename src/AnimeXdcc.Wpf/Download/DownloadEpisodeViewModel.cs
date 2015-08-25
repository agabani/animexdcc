using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf.Download
{
    internal class DownloadEpisodeViewModel : BindableBase
    {
        private readonly IAnimeXdccService _animeXdccService;
        private DownloadProgress _downloadProgress = new DownloadProgress();
        private Package _package;

        public DownloadEpisodeViewModel(IAnimeXdccService animeXdccService)
        {
            _animeXdccService = animeXdccService;

            _animeXdccService.DownloadProgressEvent += OnDownloadProgressEvent;
        }

        public Package Package
        {
            get { return _package; }
            set
            {
                SetProperty(ref _package, value);
                DownloadAsync();
            }
        }

        public DownloadProgress DownloadProgress
        {
            get { return _downloadProgress; }
            set { SetProperty(ref _downloadProgress, value); }
        }

        private void OnDownloadProgressEvent(object sender, DownloadProgress downloadProgress)
        {
            DownloadProgress = downloadProgress;
        }

        public async void DownloadAsync()
        {
            await _animeXdccService.DownloadAsync(Package.BotName, Package.Id);
        }
    }
}