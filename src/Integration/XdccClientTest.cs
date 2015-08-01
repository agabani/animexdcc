using System.IO;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Clients;
using AnimeXdcc.Core.Logging;
using Integration.Clients;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class XdccClientTest
    {
        [Test]
        public async Task Should_be_able_to_recieve_file()
        {
            var dccClient = new DccClient();

            var fileStream = File.OpenRead(@"Data\17 - Nintendo - Mute City Ver. 3.mp3");
            var sendTask = dccClient.Send(12345, fileStream, fileStream.Length);

            var xdccDccClient = new XdccDccClient(new TraceLogger(TraceLogger.Level.Debug));

            await xdccDccClient.DownloadAsync("127.0.0.1", 12345, fileStream.Length, @"17 - Nintendo - Mute City Ver. 3.mp3");
            
            await sendTask;
        }
    }
}