using System.Threading.Tasks;
using AnimeXdcc.Core;
using Integration.Bots;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class AnimeXdccClientTest
    {
        [Test]
        public async Task Should_be_able_to_download_package()
        {
            var simpleXdccBot = new SimpleXdccBot();
            var hostTask = simpleXdccBot.HostFile("speechlessdownloader");

            await Task.Delay(5000);

            var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667);
            await animeXdccClient.DownloadPackage("ObserverXdccServer", 1);

            await hostTask;
        }

        [Test]
        public async Task Should_be_able_to_download_from_any_bot()
        {
            var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667);
            await animeXdccClient.DownloadPackage("CR-HOLLAND|NEW", 5054);
        }
    }
}