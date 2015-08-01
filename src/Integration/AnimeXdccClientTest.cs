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
            const string observerxdccserver = "ObserverXdccServer";
            const string animeXdccClientNickname = "speechlessdownloader";
            const string hostname = "irc.rizon.net";

            var simpleXdccBot = new SimpleXdccBot(hostname, observerxdccserver, @"Data\17 - Nintendo - Mute City Ver. 3.mp3");
            var hostTask = simpleXdccBot.HostFile(animeXdccClientNickname);

            await Task.Delay(5000);

            var animeXdccClient = new AnimeXdccClient(hostname, 6667, animeXdccClientNickname);
            await animeXdccClient.DownloadPackage(observerxdccserver, 1);

            await hostTask;
        }

        [Test]
        public async Task Should_be_able_to_download_from_any_bot()
        {
            var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667, "speechlessdownloader");
            await animeXdccClient.DownloadPackage("CR-HOLLAND|NEW", 5054);
        }
    }
}