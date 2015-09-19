using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Notifications;

namespace AnimeXdcc.Core.Services
{
    public class DownloadQueueService : IDownloadQueueService
    {
        public delegate void DownloadEventHandler(object sender, DownloadEventArgs e);

        private readonly Queue<DownloadJob> _queuedDownloads;
        private readonly IDownloadService _service;
        private bool _processing;

        public DownloadQueueService(IDownloadService service)
        {
            _service = service;
            _processing = false;
            _queuedDownloads = new Queue<DownloadJob>();
        }

        public void AddToQueue(string fileName, List<DccPackage> sources,
            INotificationListener<DccTransferStatistic> listener)
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

                OnDownloadStarted(new DownloadEventArgs(downloadJob.FileName));

                await ProcessJob(downloadJob);

                OnDownloadTerminated(new DownloadEventArgs(downloadJob.FileName));
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

        public event DownloadEventHandler DownloadTerminated;
        public event DownloadEventHandler DownloadStarted;

        protected virtual void OnDownloadTerminated(DownloadEventArgs e)
        {
            var handler = DownloadTerminated;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDownloadStarted(DownloadEventArgs e)
        {
            var handler = DownloadStarted;
            if (handler != null) handler(this, e);
        }

        public class DownloadEventArgs : EventArgs
        {
            public DownloadEventArgs(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; private set; }
        }

        internal class DownloadJob
        {
            internal DownloadJob(string fileName, List<DccPackage> dccPackageSources,
                INotificationListener<DccTransferStatistic> listener)
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