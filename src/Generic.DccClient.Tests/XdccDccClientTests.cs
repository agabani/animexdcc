using AnimeXdcc.Common.Logging;
using NUnit.Framework;

namespace Generic.DccClient.Tests
{
    [TestFixture]
    public class XdccDccClientTests
    {
        [Test]
        public void Should_be_able_to_download_file()
        {
            var xdccDccClient = new XdccDccClient(new TraceLogger(TraceLogger.Level.Debug));

            const string ipAddress = "95.211.136.69";
            const int port = 12351;
            const string fileName = "DanMachi.mkv";
            const uint fileSize = 154208487;

            xdccDccClient.Download(ipAddress, port, fileSize, fileName);
        }

        [Test]
        public void Should_be_able_to_download_file_async()
        {
            var xdccDccClient = new XdccDccClient(new TraceLogger(TraceLogger.Level.Debug));

            const string ipAddress = "95.211.136.69";
            const int port = 12348;
            const string fileName = "DanMachi.mkv";
            const uint fileSize = 154208487;

            xdccDccClient.DownloadAsync(ipAddress, port, fileSize, fileName).GetAwaiter().GetResult();
        }
    }
}
