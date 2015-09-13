using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services.Download;

namespace AnimeXdcc.Wpf.Download
{
    public class DownloadQueueViewModel : BindableBase
    {
        private readonly IDownloadQueueService _service;
        private Episode _activeDownload;
        private ObservableCollection<Episode> _completedDownloads;
        private ObservableCollection<Episode> _queuedDownloads;

        public DownloadQueueViewModel(IDownloadQueueService service)
        {
            _service = service;
            _queuedDownloads = new ObservableCollection<Episode>();
            _completedDownloads = new ObservableCollection<Episode>();
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

        public void AddToDownloadQueue(string fileName, List<DccPackage> source)
        {
            if (IsDuplicate(fileName))
            {
                return;
            }

            var episode = new Episode(fileName);

            QueuedDownloads.Add(episode);

            // TODO: add episode to download service queue
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