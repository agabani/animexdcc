using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.HumanReadable;
using AnimeXdcc.Core.Components.Notifications;
using AnimeXdcc.Core.Services;
using AnimeXdcc.Wpf.Infrastructure.Bindable;

namespace AnimeXdcc.Wpf.Download
{
    // TODO: Add cancel buttons
    // TODO: Inject dependencies
    public class DownloadQueueViewModel : BindableBase
    {
        private readonly BytesConverter _bytesConverter = new BytesConverter();
        private readonly NotificationListener<DccTransferStatistic> _notificationListener;
        private readonly IDownloadQueueService _service;
        private Episode _activeDownload;
        private ObservableCollection<Episode> _completedDownloads;
        private ObservableCollection<Episode> _queuedDownloads;

        public DownloadQueueViewModel(IDownloadQueueService service)
        {
            _notificationListener = new NotificationListener<DccTransferStatistic>(ExecuteAsync);

            _service = service;
            _queuedDownloads = new ObservableCollection<Episode>();
            _completedDownloads = new ObservableCollection<Episode>();

            _service.DownloadStarted += ServiceOnDownloadStarted;
            _service.DownloadTerminated += ServiceOnDownloadTerminated;
        }

        public Episode ActiveDownload
        {
            get { return _activeDownload; }
            set { SetProperty(ref _activeDownload, value); }
        }

        public ObservableCollection<Episode> QueuedDownloads
        {
            get { return _queuedDownloads; }
            set { SetProperty(ref _queuedDownloads, value); }
        }

        public ObservableCollection<Episode> CompletedDownloads
        {
            get { return _completedDownloads; }
            set { SetProperty(ref _completedDownloads, value); }
        }

        private void ServiceOnDownloadTerminated(object sender, DownloadQueueService.DownloadEventArgs downloadEventArgs)
        {
            SetToCompletedDownload();
        }

        private void ServiceOnDownloadStarted(object sender, DownloadQueueService.DownloadEventArgs downloadEventArgs)
        {
            SetToActiveDownload(downloadEventArgs.FileName);
        }

        public void AddToDownloadQueue(string fileName, List<DccPackage> source)
        {
            var episode = Episode.Default(fileName);

            QueuedDownloads.Add(episode);

            _service.AddToQueue(fileName, source, _notificationListener);
        }

        private Task ExecuteAsync(DccTransferStatistic dccTransferStatistic)
        {
            ActiveDownload = ConvertEpisode(dccTransferStatistic);

            return Task.FromResult<object>(null);
        }

        public void SetToActiveDownload(string filename)
        {
            var episode = QueuedDownloads.First(d => d.FileName == filename);

            QueuedDownloads.Remove(episode);

            ActiveDownload = episode;
        }

        public void SetToCompletedDownload()
        {
            var episode = ActiveDownload;

            ActiveDownload = null;

            CompletedDownloads.Add(episode);
        }

        private Episode ConvertEpisode(DccTransferStatistic statistic)
        {
            return new StatisticsToEpisodeConverter(_bytesConverter).Convert(ActiveDownload.FileName, statistic);
        }

        public class Episode
        {
            public static Episode Default(string fileName)
            {
                return new Episode
                {
                    FileName = fileName,
                    TransferSpeedText = string.Empty,
                    PercentageComplete = 0,
                    TimeText = string.Empty,
                    TransferProgressText = string.Empty
                };
            }

            public string FileName { get; set; }
            public int PercentageComplete { get; set; }
            public string TransferProgressText { get; set; }
            public string TransferSpeedText { get; set; }
            public string TimeText { get; set; }
        }

        public class StatisticsToEpisodeConverter
        {
            private readonly IBytesConverter _bytesConverter;

            public StatisticsToEpisodeConverter(IBytesConverter bytesConverter)
            {
                _bytesConverter = bytesConverter;
            }

            public Episode Convert(string fileName, DccTransferStatistic statistic)
            {
                var bytesTransferredPerSecond = _bytesConverter.ToHumanReadable((long)statistic.BytesPerSecond);

                var transferProgressText = string.Format("{0} of {1} ({2:N1}%)",
                    _bytesConverter.ToHumanReadable(statistic.BytesTransferred),
                    _bytesConverter.ToHumanReadable(statistic.FileSize),
                    statistic.PercentageComplete);

                var transferSpeedText = string.Format("Speed: {0}/s", bytesTransferredPerSecond);

                var timeText = statistic.PercentageComplete >= 100
                    ? string.Format("Elapsed: {0} s", statistic.SecondsElapsed)
                    : string.Format("ETA: {0} s", statistic.SecondsRemaining);

                return new Episode
                {
                    FileName = fileName,
                    PercentageComplete = (int) statistic.PercentageComplete,
                    TransferProgressText = transferProgressText,
                    TransferSpeedText = transferSpeedText,
                    TimeText = timeText
                };
            }
        }
    }
}