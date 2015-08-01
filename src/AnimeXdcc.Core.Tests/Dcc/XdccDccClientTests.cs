using System.IO;
using System.Net;
using AnimeXdcc.Core.Dcc.Clients;
using AnimeXdcc.Core.Logging;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Dcc
{
    [TestFixture]
    public class XdccDccClientTests
    {
        [Test]
        public void Should_be_able_to_download_file_async()
        {
            var xdccDccClient = new DccClient();

            var fileStream = File.OpenWrite("DanMachi.mkv");

            const string ipAddress = "95.211.136.69";
            const int port = 12348;
            const long fileSize = 154208487;

            xdccDccClient.DownloadAsync(fileStream, IPAddress.Parse(ipAddress), port, fileSize, 0).GetAwaiter().GetResult();
        }
    }
}