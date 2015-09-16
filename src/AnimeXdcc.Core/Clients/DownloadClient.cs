using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Dcc.Components;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Irc.Components;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Components.Notifications;
using AnimeXdcc.Core.Components.Parsers.Dcc;

namespace AnimeXdcc.Core.Clients
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
        private readonly IDccMessageParser _dccMessageParser;
        private IIrcClient _ircClient;

        public DownloadClient(IIrcClient ircClient, IDccClientFactory dccClientFactory,
            IDccMessageParser dccMessageParser)
        {
            _ircClient = ircClient;
            _dccClientFactory = dccClientFactory;
            _dccMessageParser = dccMessageParser;
        }

        public async Task<DownloadResult> DownloadAsync(DccPackage package, IStreamProvider provider,
            INotificationListener<DccTransferStatistic> listener)
        {
            var ircResult = await RequestFromSourceAsync(package);

            if (!ircResult.Successful)
            {
                return FailureDownloadResult(ircResult);
            }

            var message = _dccMessageParser.Parse(ircResult.Result);

            return await DownloadFromSourceAsync(message, provider, listener);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private Task<IrcClient.IrcResult> RequestFromSourceAsync(DccPackage package)
        {
            return _ircClient.RequestPackageAsync(package.BotName, package.PackageId);
        }

        private async Task<DownloadResult> DownloadFromSourceAsync(DccMessageParser.DccSendMessage message,
            IStreamProvider provider,
            INotificationListener<DccTransferStatistic> listener)
        {
            using (var dccClient = _dccClientFactory.Create(message.FileSize))
            {
                using (var stream = provider.GetStream(message.FileName, StreamProvider.Strategy.Overwrite))
                {
                    DccTransferStatistic statistic = null;

                    DccClient.TransferProgressEventHandler dccClientOnTransferProgress = (sender, args) =>
                    {
                        statistic = args.Statistic;
                        listener.Notify(args.Statistic);
                    };

                    dccClient.TransferProgress += dccClientOnTransferProgress;
                    var dccResult =
                        await dccClient.DownloadAsync(message.IpAddress, message.Port, message.FileSize, stream);
                    dccClient.TransferProgress -= dccClientOnTransferProgress;

                    if (!dccResult.Successful)
                    {
                        return new DownloadResult(false, DownloadFailureKind.SourceNotAvailable);
                    }

                    return CreateResult(statistic);
                }
            }
        }

        private static DownloadResult FailureDownloadResult(IrcClient.IrcResult ircResult)
        {
            switch (ircResult.FailureKind)
            {
                case IrcClient.IrcFailureKind.None:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.None);
                case IrcClient.IrcFailureKind.TaskCancelled:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.DownloadTerminated);
                case IrcClient.IrcFailureKind.ServerNotFound:
                    return new DownloadResult(ircResult.Successful, DownloadFailureKind.ServerNotAvailable);
                case IrcClient.IrcFailureKind.SourceNotFound:
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_ircClient != null)
                {
                    _ircClient.Dispose();
                    _ircClient = null;
                }
            }
        }

        ~DownloadClient()
        {
            Dispose(false);
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