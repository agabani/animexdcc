using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Components.Notifications;

namespace AnimeXdcc.Core.Clients
{
    public interface IDownloadClient
    {
        Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package, IStreamProvider provider,
            INotificationListener<DccTransferStatistic> listener);
    }
}