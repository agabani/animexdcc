using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Components;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Core.Irc.DccMessage;
using AnimeXdcc.Core.Utilities;
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
        private readonly IDccClient _dccClient;

        public DownloadClient(IXdccIrcClient ircClient, IDccClient dccClient)
        {
            _ircClient = ircClient;
            _dccClient = dccClient;
        }

        public async Task<DownloadResult> DownloadAsync(DccPackage package/*, IStreamProvider provider*/)
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

            var dccMessage = new DccMessageParser(new IpConverter()).Parse(ircResult.Result);

            //TODO: How to give location of where to save file...

            //_dccClient.DownloadAsync(dccMessage.IpAddress, dccMessage.Port, dccMessage.FileSize, provider.GetStream(dccMessage.FileName) );
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