using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public interface IDownloadClient
    {
        Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package, IStreamProvider provider,
            INotificationListener<DccTransferStatistic> listener);
    }
}