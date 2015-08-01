using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Clients;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Core.Irc.DccMessage;
using AnimeXdcc.Core.Utilities;

namespace AnimeXdcc.Core
{
    public class AnimeXdccClient
    {
        private readonly XdccDccClient _xdccDccClient;
        private readonly XdccIrcClient _xdccIrcClient;

        public AnimeXdccClient(string hostname, int port, string nickname)
        {
            _xdccIrcClient = new XdccIrcClient(hostname, port, nickname);
            _xdccDccClient = new XdccDccClient();
        }

        public async Task DownloadPackage(string target, int packageId)
        {
            var requestPackage = await _xdccIrcClient.RequestPackageAsync(target, packageId);
            Console.WriteLine("Package request accepted");

            var dccMessage = new DccMessageParser(new IpConverter()).Parse(requestPackage);
            Console.WriteLine("IP Address: {0}\n" +
                              "Port: {1}\n" +
                              "File Name: {2}\n" +
                              "File Size: {3:N0}",
                dccMessage.IpAddress, dccMessage.Port, dccMessage.FileName, dccMessage.FileSize);

            var fileStream = File.OpenWrite(dccMessage.FileName);

            _xdccDccClient.TransferStatus += (sender, status) =>
            {
                Console.WriteLine("{0:N0}/{1:N0} [{2:N2}%] @ {3:N0} KB/s [{4:N1}/{5:N1}]",
                    status.DownloadedBytes,
                    status.FileSize,
                    (double)status.DownloadedBytes / (double)status.FileSize * 100,
                    status.BytesPerMillisecond,
                    status.ElapsedTime / 1000,
                    (status.FileSize / status.BytesPerMillisecond) / 1000);
            };

            var dccTransferStatus = await _xdccDccClient.DownloadAsync(fileStream, IPAddress.Parse(dccMessage.IpAddress), dccMessage.Port,
                dccMessage.FileSize, 0);

            Console.WriteLine("Download complete...\n" +
                              "{0:N0} @ {1:N0} KB/s [{2:N1} ms]",
                              dccTransferStatus.FileSize,
                              dccTransferStatus.BytesPerMillisecond,
                              dccTransferStatus.ElapsedTime / 1000);
        }
    }
}