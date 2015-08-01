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

        public AnimeXdccClient(string hostname, int port)
        {
            _xdccIrcClient = new XdccIrcClient(hostname, port);
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
                              "File Size: {3}",
                dccMessage.IpAddress, dccMessage.Port, dccMessage.FileName, dccMessage.FileSize);

            var fileStream = File.OpenWrite(dccMessage.FileName);

            _xdccDccClient.TransferStatus += (sender, status) =>
            {
                Console.WriteLine("{0}/{1} @ {2} [{3}/{4}]",
                    status.DownloadedBytes, status.FileSize, status.BytesPerMillisecond, status.ElapsedTime,
                    status.FileSize / status.BytesPerMillisecond);
            };

            var dccTransferStatus = await _xdccDccClient.DownloadAsync(fileStream, IPAddress.Parse(dccMessage.IpAddress), dccMessage.Port,
                dccMessage.FileSize, 0);

            Console.WriteLine("Download complete...\n" +
                              "{0} @ {1} KB/s [{2} ms]", dccTransferStatus.FileSize, dccTransferStatus.BytesPerMillisecond, dccTransferStatus.ElapsedTime);
        }
    }
}