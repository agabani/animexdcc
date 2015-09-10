using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public interface IDownloadService
    {
        Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package, INotificationListener<DccTransferStatistic> listener);
    }
}