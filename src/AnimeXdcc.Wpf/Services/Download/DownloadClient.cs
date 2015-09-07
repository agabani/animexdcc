using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public class DownloadClient : IDownloadClient
    {
        private XdccIrcClient _ircClient;

        public DownloadClient(XdccIrcClient ircClient)
        {
            _ircClient = ircClient;
        }

        public Task<DownloadResult> DownloadAsync(DccPackage dccPackage)
        {
            throw new NotImplementedException();
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

        public enum DownloadFailureKind
        {
            ServerNotAvailable,
            SourceNotAvailable,
            DownloadTerminated
        }
    }
}