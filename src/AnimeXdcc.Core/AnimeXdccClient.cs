using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Publishers.Download;
using AnimeXdcc.Core.Dcc.Clients;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Core.Irc.DccMessage;
using AnimeXdcc.Core.SystemWrappers.Timer;
using AnimeXdcc.Core.Utilities;

namespace AnimeXdcc.Core
{
    public class AnimeXdccClient : IAnimeXdccClient
    {
        private XdccIrcClient _xdccIrcClient;

        public AnimeXdccClient(string hostname, int port, string nickname)
        {
            _xdccIrcClient = new XdccIrcClient(hostname, port, nickname);
        }

        public async Task<DccTransferStatus> DownloadPackageAsync(string target, int packageId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestPackage = await RequestPackageAsync(target, packageId, cancellationToken);
            var dccMessage = new DccMessageParser(new IpConverter()).Parse(requestPackage);

            DccTransferStatus dccTransferStatus;

            using (var fileStream = File.OpenWrite(dccMessage.FileName))
            {
                dccTransferStatus = await DownloadAsync(fileStream, dccMessage, cancellationToken);
            }

            return dccTransferStatus;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<DccTransferStatus> TransferStatusEvent;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_xdccIrcClient != null)
                {
                    _xdccIrcClient.Dispose();
                    _xdccIrcClient = null;
                }
            }
        }

        ~AnimeXdccClient()
        {
            Dispose(false);
        }

        private async Task<DccTransferStatus> DownloadAsync(Stream stream, DccSendMessage dccMessage,
            CancellationToken cancellationToken)
        {
            DccTransferStatus dccTransferStatus;
            using (var xdccDccClient = XdccDccClient())
            {
                dccTransferStatus = await xdccDccClient.DownloadAsync(stream,
                    IPAddress.Parse(dccMessage.IpAddress), dccMessage.Port,
                    dccMessage.FileSize, 0, cancellationToken);
            }
            return dccTransferStatus;
        }

        private XdccDccClient XdccDccClient()
        {
            var xdccDccClient = new XdccDccClient(new DccDownloadStatusPublisher(new TimerWrapper(250)));
            xdccDccClient.TransferStatus += (sender, status) => { OnTransferStatusEvent(status); };
            return xdccDccClient;
        }

        private async Task<string> RequestPackageAsync(string target, int packageId, CancellationToken cancellationToken)
        {
            return await _xdccIrcClient.RequestPackageAsync(target, packageId, cancellationToken);
        }

        protected virtual void OnTransferStatusEvent(DccTransferStatus e)
        {
            var handler = TransferStatusEvent;
            if (handler != null) handler(this, e);
        }
    }
}