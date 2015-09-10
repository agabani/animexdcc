using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AnimeXdcc.Core.Components.HumanReadable;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services.Download;

namespace AnimeXdcc.Wpf.Download
{
    public class DownloadQueueViewModel : BindableBase
    {
        private readonly IDownloadQueueService _service;
        public ObservableCollection<EpisodeDownload> Downloads;

        public DownloadQueueViewModel(IDownloadQueueService service)
        {
            _service = service;
            Downloads = new ObservableCollection<EpisodeDownload>();
        }

        public void AddToDownloadQueue(string fileName, List<DccPackage> source)
        {
            var download = new EpisodeDownload(fileName);

            Downloads.Add(download);

            var notificationListener =
                new NotificationListener<DccTransferStatistic>(
                    statistic => { return new Task(() => { download.Parse(statistic); }); });

            _service.AddToQueue(fileName, source, notificationListener);
        }

        public class EpisodeDownload
        {
            public EpisodeDownload(string fileName)
            {
                FileName = fileName;
            }

            public string RemainingBytes { get; set; }
            public string TransferedBytes { get; set; }
            public string TransferSpeed { get; set; }
            public string TimeElapsed { get; set; }
            public string TimeRemaining { get; set; }
            public string FileName { get; private set; }
            public string FileSize { get; set; }
            public string PercentageComplete { get; set; }
            public long BytesTransferred { get; set; }
            public double Percentage { get; set; }

            public void Parse(DccTransferStatistic statistic)
            {
                var convertor = new BytesConvertor();

                Percentage = statistic.PercentageComplete;
                FileSize = string.Format("{0}", convertor.ToHumanReadable(statistic.FileSize));
                PercentageComplete = string.Format("{0}%", statistic.PercentageComplete);
                TimeRemaining = string.Format("{0}s", statistic.SecondsRemaining);
                TimeElapsed = string.Format("{0}s", statistic.SecondsElapsed);
                TransferSpeed = string.Format("{0}/s", convertor.ToHumanReadable((long) statistic.BytesPerSecond));
                TransferedBytes = string.Format("{0}", convertor.ToHumanReadable(statistic.BytesTransferred));
                RemainingBytes = string.Format("{0}", convertor.ToHumanReadable(statistic.BytesRemaining));
            }
        }
    }
}