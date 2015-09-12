using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Components.HumanReadable;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services.Download;

namespace AnimeXdcc.Wpf.Download
{
    internal class DownloadEpisodeViewModel : BindableBase
    {
        private DownloadProgress _downloadProgress = new DownloadProgress();
        private DownloadProgressHumanReadable _downloadProgressHumanReadable = new DownloadProgressHumanReadable(new DownloadProgress());
        private Package _package;

        private readonly IDownloadService _service;
        private readonly INotificationListener<DccTransferStatistic> _notificationListener;

        public DownloadEpisodeViewModel(IDownloadService service)
        {
            _service = service;
            _notificationListener = new NotificationListener<DccTransferStatistic>(ExecuteAsync);
        }

        private Task ExecuteAsync(DccTransferStatistic statistic)
        {
            var downloadProgress = new DownloadProgress
            {
                PercentageComplete = (int)statistic.PercentageComplete,
                RemainingBytes = statistic.BytesRemaining,
                TimeElapsed = TimeSpan.FromSeconds(statistic.SecondsElapsed),
                TimeRemaining = TimeSpan.FromSeconds(statistic.BytesRemaining),
                TotalBytes = statistic.FileSize,
                TransferedBytes = statistic.BytesTransferred,
                TransferSpeed = (long)statistic.BytesPerSecond
            };

            DownloadProgress = downloadProgress;

            DownloadProgressHumanReadable = new DownloadProgressHumanReadable(DownloadProgress);

            return Task.FromResult<object>(null);
        }

        public Package Package
        {
            get { return _package; }
            set
            {
                SetProperty(ref _package, value);
            }
        }

        public void Download(DccPackage dccPackage)
        {
            _service.DownloadAsync(dccPackage, _notificationListener).GetAwaiter();

            Package = new Package
            {
                BotName = dccPackage.BotName,
                FileName = dccPackage.FileName,
                FileSize = dccPackage.FileSize,
                Id = dccPackage.PackageId
            };
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