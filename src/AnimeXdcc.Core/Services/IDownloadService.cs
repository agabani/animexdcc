using System.Threading.Tasks;
using AnimeXdcc.Core.Clients;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Notifications;

namespace AnimeXdcc.Core.Services
{
    public interface IDownloadService
    {
        Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package, INotificationListener<DccTransferStatistic> listener);
    }
}