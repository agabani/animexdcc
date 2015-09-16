using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Components.Notifications;

namespace AnimeXdcc.Core.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly IStreamProvider _streamProvider;
        private IDownloadClient _client;

        public DownloadService(IDownloadClient client, IStreamProvider streamProvider)
        {
            _client = client;
            _streamProvider = streamProvider;
        }

        public async Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package,
            INotificationListener<DccTransferStatistic> listener)
        {
            return await _client.DownloadAsync(package, _streamProvider, listener);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        ~DownloadService()
        {
            Dispose(false);
        }
    }
}