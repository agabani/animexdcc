using System.Collections.Generic;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Notifications;

namespace AnimeXdcc.Core.Services
{
    public interface IDownloadQueueService
    {
        void AddToQueue(string fileName, List<DccPackage> sources, INotificationListener<DccTransferStatistic> listener);
        event DownloadQueueService.DownloadEventHandler DownloadTerminated;
        event DownloadQueueService.DownloadEventHandler DownloadStarted;
    }
}