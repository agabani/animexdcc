using System.Collections.Generic;
using System.Threading.Tasks;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public class DownloadService
    {
        private readonly List<DownloadJob> _activeDownloads;
        private readonly IDownloadClient _client;
        private readonly Queue<DownloadJob> _queuedDownloads;
        private bool _processing;

        public DownloadService(IDownloadClient client)
        {
            _client = client;
            _processing = false;
            _activeDownloads = new List<DownloadJob>();
            _queuedDownloads = new Queue<DownloadJob>();
        }

        public void AddToQueue(string fileName, List<DccPackage> sources)
        {
            _queuedDownloads.Enqueue(new DownloadJob(fileName, sources));
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
                var result = await _client.DownloadAsync(package);

                if (result.Successful)
                {
                    return true;
                }
            }

            return false;
        }

        internal class DownloadJob
        {
            internal DownloadJob(string fileName, List<DccPackage> dccPackageSources)
            {
                FileName = fileName;
                DccPackageSources = dccPackageSources;
            }

            internal string FileName { get; private set; }
            internal List<DccPackage> DccPackageSources { get; private set; }
        }
    }
}