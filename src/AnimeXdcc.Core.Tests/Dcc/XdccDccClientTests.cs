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
            var xdccDccClient = new XdccDccClient(new TraceLogger(TraceLogger.Level.Debug));

            const string ipAddress = "95.211.136.69";
            const int port = 12348;
            const string fileName = "DanMachi.mkv";
            const long fileSize = 154208487;

            xdccDccClient.DownloadAsync(ipAddress, port, fileSize, fileName).GetAwaiter().GetResult();
        }
    }
}