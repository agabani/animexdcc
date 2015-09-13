using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Dcc.Components;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Core.Irc.DccMessage;
using AnimeXdcc.Core.Utilities;
using AnimeXdcc.Wpf.Infrastructure.Notifications;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public class DownloadClient : IDownloadClient
    {
        public enum DownloadFailureKind
        {
            None,
            ServerNotAvailable,
            SourceNotAvailable,
            DownloadTerminated
        }

        private readonly IDccClientFactory _dccClientFactory;
        private readonly IXdccIrcClient _ircClient;

        public DownloadClient(IXdccIrcClient ircClient, IDccClientFactory dccClientFactory)
        {
            _ircClient = ircClient;
            _dccClientFactory = dccClientFactory;
        }

        public async Task<DownloadResult> DownloadAsync(DccPackage package, IStreamProvider provider,
            INotificationListener<DccTransferStatistic> listener)
        {
            var ircResult = await _ircClient.RequestPackageAsync(package.BotName, package.PackageId);

            if (!ircResult.Successful)
            {
                return FailureDownloadResult(ircResult);
            }

            // TODO: move this message parser some where more responsible
            var message = new DccMessageParser(new IpConverter()).Parse(ircResult.Result);

            var dccClient = _dccClientFactory.Create(message.FileSize);

            DccTransferStatistic statistic = null;

            dccClient.TransferProgress += (sender, args) =>
            {
                statistic = args.Statistic;
                listener.Notify(args.Statistic);
            };

            using (var stream = provider.GetStream(package.FileName, StreamProvider.Strategy.Overwrite))
            {
                await dccClient.DownloadAsync(message.IpAddress, message.Port, message.FileSize, stream);
            }

            return CreateResult(statistic);
        }

        private static DownloadResult FailureDownloadResult(XdccIrcClient.IrcResult ircResult)
        {
            switch (ircResult.FailureKind)
            {
                case XdccIrcClient.IrcFailureKind.None:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.None);
                case XdccIrcClient.IrcFailureKind.TaskCancelled:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.DownloadTerminated);
                case XdccIrcClient.IrcFailureKind.ServerNotFound:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.ServerNotAvailable);
                case XdccIrcClient.IrcFailureKind.SourceNotFound:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.SourceNotAvailable);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static DownloadResult CreateResult(DccTransferStatistic statistic)
        {
            DownloadResult result;

            if (statistic == null)
            {
                result = new DownloadResult(false, DownloadFailureKind.SourceNotAvailable);
            }
            else
            {
                if (statistic.BytesTransferred == statistic.FileSize)
                {
                    result = new DownloadResult(true, DownloadFailureKind.None);
                }
                else if (statistic.BytesTransferred > 0)
                {
                    result = new DownloadResult(false, DownloadFailureKind.DownloadTerminated);
                }
                else
                {
                    result = new DownloadResult(false, DownloadFailureKind.SourceNotAvailable);
                }
            }

            return result;
        }

        public class DownloadResult
        {
            public DownloadResult(bool successful, DownloadFailureKind failure)
            {
                Successful = successful;
                Failure = failure;
            }

            internal bool Successful { get; private set; }
            internal DownloadFailureKind Failure { get; private set; }
        }
    }
}