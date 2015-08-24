using System;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Dcc.Models;

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

        private void OnTransferStatusEvent(object sender, DccTransferStatus dccTransferStatus)
        {
            var progress = Calculate(dccTransferStatus);
            OnTransferStatusEvent(progress);
        }

        private DownloadProgress Calculate(DccTransferStatus status)
        {
            return new DownloadProgress
            {
                PercentageComplete = (int)(status.DownloadedBytes * 100 / status.FileSize)
            };
        }

        public event EventHandler<DownloadProgress> DownloadProgressEvent;

        protected virtual void OnTransferStatusEvent(DownloadProgress e)
        {
            var handler = DownloadProgressEvent;
            if (handler != null) handler(this, e);
        }
    }

    internal class DownloadProgress
    {
        public int PercentageComplete { get; set; }
        public string TimeRemaining { get; set; }
        public string TimeElapsed { get; set; }
        public string TransferSpeed { get; set; }
        public string TransferedBytes { get; set; }
        public string RemainingBytes { get; set; }
        public string TotalBytes { get; set; }
    }
}