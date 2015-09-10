using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public class DownloadService : IDownloadService
    {
        private readonly IDownloadClient _client;
        private readonly IStreamProvider _streamProvider;

        public DownloadService(IDownloadClient client, IStreamProvider streamProvider)
        {
            _client = client;
            _streamProvider = streamProvider;
        }

        public async Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package, INotificationListener<DccTransferStatistic> listener)
        {
            return await _client.DownloadAsync(package, _streamProvider, listener);
        }
    }
}
