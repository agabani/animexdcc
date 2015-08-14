using System;
using System.IO;
using System.Net;
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
    public class AnimeXdccClient : IDisposable
    {
        private readonly XdccIrcClient _xdccIrcClient;

        public AnimeXdccClient(string hostname, int port, string nickname)
        {
            _xdccIrcClient = new XdccIrcClient(hostname, port, nickname);
        }

        public void Dispose()
        {
            _xdccIrcClient.Dispose();
        }

        public async Task<DccTransferStatus> DownloadPackage(string target, int packageId)
        {
            var requestPackage = await RequestPackage(target, packageId);
            var dccMessage = new DccMessageParser(new IpConverter()).Parse(requestPackage);
            Display(dccMessage);

            DccTransferStatus dccTransferStatus;

            using (var fileStream = File.OpenWrite(dccMessage.FileName))
            {
                dccTransferStatus = await DownloadAsync(fileStream, dccMessage);
            }

            Display(dccTransferStatus);

            return dccTransferStatus;
        }

        private static async Task<DccTransferStatus> DownloadAsync(Stream stream, DccSendMessage dccMessage)
        {
            DccTransferStatus dccTransferStatus;
            using (var xdccDccClient = XdccDccClient())
            {
                dccTransferStatus = await xdccDccClient.DownloadAsync(stream,
                    IPAddress.Parse(dccMessage.IpAddress), dccMessage.Port,
                    dccMessage.FileSize, 0);
            }
            return dccTransferStatus;
        }

        private static XdccDccClient XdccDccClient()
        {
            var xdccDccClient = new XdccDccClient(new DccDownloadStatusPublisher(new TimerWrapper(1000)));
            xdccDccClient.TransferStatus += XdccDccClientOnTransferStatus;
            return xdccDccClient;
        }

        private async Task<string> RequestPackage(string target, int packageId)
        {
            return await _xdccIrcClient.RequestPackageAsync(target, packageId);
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