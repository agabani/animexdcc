using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Irc.Clients;
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

        private readonly IXdccIrcClient _ircClient;

        public DownloadClient(IXdccIrcClient ircClient)
        {
            _ircClient = ircClient;
        }

        public async Task<DownloadResult> DownloadAsync(DccPackage package)
        {
            var ircResult = await _ircClient.RequestPackageAsync(package.BotName, package.PackageId);

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