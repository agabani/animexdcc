using System.Collections.Generic;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public interface IDownloadQueueService
    {
        void AddToQueue(string fileName, List<DccPackage> sources, INotificationListener<DccTransferStatistic> listener);
        event DownloadQueueService.DownloadEventHandler DownloadTerminated;
        event DownloadQueueService.DownloadEventHandler DownloadStarted;
    }
}