using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Notifications;
using AnimeXdcc.Core.Services;
using AnimeXdcc.Wpf.Infrastructure.Bindable;

namespace AnimeXdcc.Wpf.Download
{
    public class DownloadQueueViewModel : BindableBase
    {
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
            if (IsDuplicate(fileName))
            {
                return;
            }

            var episode = new Episode(fileName);

            QueuedDownloads.Add(episode);

            // TODO: add episode to download service queue
            _service.AddToQueue(fileName, source, _notificationListener);
        }

        private Task ExecuteAsync(DccTransferStatistic dccTransferStatistic)
        {
            ActiveDownload = new Episode(ActiveDownload.FileName)
            {
                PercentageComplete = (int)dccTransferStatistic.PercentageComplete
            }; 

            return Task.FromResult<object>(null);
        }

        private bool IsDuplicate(string fileName)
        {
            return QueuedDownloads.Any(n => n.FileName == fileName) ||
                   (ActiveDownload != null && ActiveDownload.FileName == fileName) ||
                   CompletedDownloads.Any(n => n.FileName == fileName);
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

        public class Episode
        {
            public Episode(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; private set; }
            public int PercentageComplete { get; set; }
        }
    }
}