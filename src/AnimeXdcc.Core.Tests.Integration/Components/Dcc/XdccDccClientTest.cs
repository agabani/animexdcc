﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Clients;
using Integration.Clients;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Integration.Components.Dcc
{
    [TestFixture]
    public class XdccDccClientTest
    {
        [Test]
        public async Task Should_be_able_to_recieve_file()
        {
            var fileStream1 = File.OpenRead(@"Data\17 - Nintendo - Mute City Ver. 3.mp3");
            var fileStream2 = File.OpenRead(@"Data\17 - Nintendo - Mute City Ver. 3.mp3");
            var sendTask1 = new IntegrationDccClient().Send(12345, fileStream1, fileStream1.Length);
            var sendTask2 = new IntegrationDccClient().Send(12346, fileStream2, fileStream2.Length);

            var xdccDccClient = new XdccDccClient();

            xdccDccClient.TransferStatus += (sender, status) =>
            {
                Console.WriteLine("{0}/{1} @ {2} KB/s [{3} ms]",status.DownloadedBytes, status.FileSize, status.BytesPerMillisecond, status.ElapsedTime);
            };

            var dccTransferStatus = await xdccDccClient.DownloadAsync(File.OpenWrite(@"17 - Nintendo - Mute City Ver. 3_1.mp3"), IPAddress.Parse("127.0.0.1"), 12345, fileStream1.Length, 0);
            Console.WriteLine("{0}/{1} @ {2} KB/s [{3} ms]", dccTransferStatus.DownloadedBytes, dccTransferStatus.FileSize, dccTransferStatus.BytesPerMillisecond, dccTransferStatus.ElapsedTime);
            dccTransferStatus = await xdccDccClient.DownloadAsync(File.OpenWrite(@"17 - Nintendo - Mute City Ver. 3_2.mp3"), IPAddress.Parse("127.0.0.1"), 12346, fileStream1.Length, 0);
            Console.WriteLine("{0}/{1} @ {2} KB/s [{3} ms]", dccTransferStatus.DownloadedBytes, dccTransferStatus.FileSize, dccTransferStatus.BytesPerMillisecond, dccTransferStatus.ElapsedTime);

            await sendTask1;
            await sendTask2;
        }
    }
}