using System;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services
{
    internal class AnimeXdccService : IAnimeXdccService
    {
        private readonly IAnimeXdccClient _animeXdccClient;

        public AnimeXdccService(IAnimeXdccClient animeXdccClient)
        {
            _animeXdccClient = animeXdccClient;
            _animeXdccClient.TransferStatusEvent += OnTransferStatusEvent;
        }

        public async Task DownloadAsync(string bot, int packageId)
        {
            var result = await _animeXdccClient.DownloadPackageAsync(bot, packageId);
            OnTransferStatusEvent(Calculate(result));
        }

        public event EventHandler<DownloadProgress> DownloadProgressEvent;

        private void OnTransferStatusEvent(object sender, DccTransferStatus dccTransferStatus)
        {
            var progress = Calculate(dccTransferStatus);
            OnTransferStatusEvent(progress);
        }

        private DownloadProgress Calculate(DccTransferStatus status)
        {
            var totalBytes = status.FileSize;

            var downloadedBytes = status.DownloadedBytes;

            var transferSpeed = (long)(status.BytesPerMillisecond * 1000);

            var remainingBytes = status.FileSize - status.DownloadedBytes;

            var percentageComplete = (int)(status.DownloadedBytes * 100 / status.FileSize);

            var timeElapsed = new TimeSpan(0, 0, 0, (int)status.ElapsedTime);

            var timeRemaining = new TimeSpan(0, 0, 0,(int)(remainingBytes / transferSpeed));

            return new DownloadProgress
            {
                PercentageComplete = percentageComplete,
                TimeElapsed = timeElapsed,
                TimeRemaining = timeRemaining,
                RemainingBytes = remainingBytes,
                TotalBytes = totalBytes,
                TransferedBytes = downloadedBytes,
                TransferSpeed = transferSpeed
            };
        }

        protected virtual void OnTransferStatusEvent(DownloadProgress e)
        {
            var handler = DownloadProgressEvent;
            if (handler != null) handler(this, e);
        }
    }
}