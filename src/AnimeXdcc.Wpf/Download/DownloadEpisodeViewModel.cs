using System;
using AnimeXdcc.Core.Components.HumanReadable;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf.Download
{
    internal class DownloadEpisodeViewModel : BindableBase
    {
        private readonly IAnimeXdccService _animeXdccService;
        private DownloadProgress _downloadProgress = new DownloadProgress();
        private DownloadProgressHumanReadable _downloadProgressHumanReadable = new DownloadProgressHumanReadable(new DownloadProgress());
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
            set
            {
                SetProperty(ref _downloadProgress, value);
                DownloadProgressHumanReadable = new DownloadProgressHumanReadable(DownloadProgress);
            }
        }

        public DownloadProgressHumanReadable DownloadProgressHumanReadable
        {
            get { return _downloadProgressHumanReadable; }
            set { SetProperty(ref _downloadProgressHumanReadable, value); }
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

    internal class DownloadProgressHumanReadable
    {
        internal DownloadProgressHumanReadable(DownloadProgress progress)
        {
            var converter = new BytesConvertor();
            PercentageComplete = string.Format("{0}%", progress.PercentageComplete);
            TimeRemaining = string.Format("{0}s", progress.TimeRemaining.TotalSeconds);
            TimeElapsed = string.Format("{0}s", progress.TimeElapsed.TotalSeconds);
            TransferSpeed = string.Format("{0}/s", converter.ToHumanReadable(progress.TransferSpeed));
            TransferedBytes = string.Format("{0}", converter.ToHumanReadable(progress.TransferedBytes));
            RemainingBytes = string.Format("{0}", converter.ToHumanReadable(progress.RemainingBytes));
            TotalBytes = string.Format("{0}", converter.ToHumanReadable(progress.TotalBytes));
        }

        public string PercentageComplete { get; set; }
        public string TimeRemaining { get; set; }
        public string TimeElapsed { get; set; }
        public string TransferSpeed { get; set; }
        public string TransferedBytes { get; set; }
        public string RemainingBytes { get; set; }
        public string TotalBytes { get; set; }
    }
}