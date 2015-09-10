using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public class DownloadQueueService : IDownloadQueueService
    {
        private readonly List<DownloadJob> _activeDownloads;
        private readonly IDownloadService _service;
        private readonly Queue<DownloadJob> _queuedDownloads;
        private bool _processing;

        public DownloadQueueService(IDownloadService service)
        {
            _service = service;
            _processing = false;
            _activeDownloads = new List<DownloadJob>();
            _queuedDownloads = new Queue<DownloadJob>();
        }

        public void AddToQueue(string fileName, List<DccPackage> sources, INotificationListener<DccTransferStatistic> listener)
        {
            _queuedDownloads.Enqueue(new DownloadJob(fileName, sources, listener));
            Process();
        }

        private async void Process()
        {
            if (_processing)
            {
                return;
            }

            _processing = true;

            try
            {
                await ProcessQueue();
            }
            finally
            {
                _processing = false;
            }
        }

        private async Task ProcessQueue()
        {
            while (_queuedDownloads.Count > 0)
            {
                var downloadJob = _queuedDownloads.Dequeue();

                _activeDownloads.Add(downloadJob);

                var success = await ProcessJob(downloadJob);

                _activeDownloads.Remove(downloadJob);
            }
        }

        private async Task<bool> ProcessJob(DownloadJob downloadJob)
        {
            var sources = downloadJob.DccPackageSources;

            foreach (var package in sources)
            {
                var result = await _service.DownloadAsync(package, downloadJob.Listener);

                if (result.Successful)
                {
                    return true;
                }
            }

            return false;
        }

        internal class DownloadJob
        {
            internal DownloadJob(string fileName, List<DccPackage> dccPackageSources, INotificationListener<DccTransferStatistic> listener)
            {
                FileName = fileName;
                DccPackageSources = dccPackageSources;
                Listener = listener;
            }

            internal string FileName { get; private set; }
            internal List<DccPackage> DccPackageSources { get; private set; }
            public INotificationListener<DccTransferStatistic> Listener { get; set; }
        }
    }
}