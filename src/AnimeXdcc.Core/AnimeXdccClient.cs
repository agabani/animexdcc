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
using AnimeXdcc.Core.SystemWrappers;
using AnimeXdcc.Core.Utilities;

namespace AnimeXdcc.Core
{
    public class AnimeXdccClient : IAnimeXdccClient
    {
        private readonly XdccIrcClient _xdccIrcClient;

        public AnimeXdccClient(string hostname, int port, string nickname)
        {
            _xdccIrcClient = new XdccIrcClient(hostname, port, nickname);
        }

        public async Task<DccTransferStatus> DownloadPackageAsync(string target, int packageId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestPackage = await RequestPackageAsync(target, packageId, cancellationToken);
            var dccMessage = new DccMessageParser(new IpConverter()).Parse(requestPackage);
            Display(dccMessage);

            DccTransferStatus dccTransferStatus;

            using (var fileStream = File.OpenWrite(dccMessage.FileName))
            {
                dccTransferStatus = await DownloadAsync(fileStream, dccMessage, cancellationToken);
            }

            Display(dccTransferStatus);

            return dccTransferStatus;
        }

        public void Dispose()
        {
            ((IDisposable) _xdccIrcClient).Dispose();
        }

        private static async Task<DccTransferStatus> DownloadAsync(Stream stream, DccSendMessage dccMessage,
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

        private static XdccDccClient XdccDccClient()
        {
            var xdccDccClient = new XdccDccClient(new DccDownloadStatusPublisher(new TimerWrapper(1000)));
            xdccDccClient.TransferStatus += XdccDccClientOnTransferStatus;
            return xdccDccClient;
        }

        private async Task<string> RequestPackageAsync(string target, int packageId, CancellationToken cancellationToken)
        {
            return await _xdccIrcClient.RequestPackageAsync(target, packageId, cancellationToken);
        }

        private static void XdccDccClientOnTransferStatus(object sender, DccTransferStatus status)
        {
            Console.WriteLine("{0:N0}/{1:N0} [{2:N2}%] @ {3:N0} KB/s [{4:N1}/{5:N1}]",
                status.DownloadedBytes,
                status.FileSize,
                (double) status.DownloadedBytes/(double) status.FileSize*100,
                status.BytesPerMillisecond,
                status.ElapsedTime/1000,
                (status.FileSize/status.BytesPerMillisecond)/1000);
        }

        private static void Display(DccTransferStatus dccTransferStatus)
        {
            Console.WriteLine("Download complete...\n" +
                              "{0:N0} @ {1:N0} KB/s [{2:N1} s]",
                dccTransferStatus.FileSize,
                dccTransferStatus.BytesPerMillisecond,
                dccTransferStatus.ElapsedTime/1000);
        }

        private static void Display(DccSendMessage dccMessage)
        {
            Console.WriteLine("IP Address: {0}\n" +
                              "Port: {1}\n" +
                              "File Name: {2}\n" +
                              "File Size: {3:N0}",
                dccMessage.IpAddress, dccMessage.Port, dccMessage.FileName, dccMessage.FileSize);
        }
    }
}