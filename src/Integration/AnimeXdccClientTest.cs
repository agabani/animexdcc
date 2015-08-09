using System;
using System.Linq;
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
        public async Task Should_be_able_to_download_package_from_internal_bot()
        {
            var configuration = new Configuration();
            var hostname = configuration.IrcServer.HostName;
            var port = configuration.IrcServer.Port;

            var serverNick = "internal" + RandomString();
            var clientNick = "client" + RandomString();

            var simpleXdccBot = new SimpleXdccBot(serverNick, hostname, port, @"Data\17 - Nintendo - Mute City Ver. 3.mp3");
            var hostTask = simpleXdccBot.HostFile(clientNick);

            await Task.Delay(5000);

            var animeXdccClient = new AnimeXdccClient(hostname, port, clientNick);
            await animeXdccClient.DownloadPackage(serverNick, 1);

            await hostTask;
        }

        [Test]
        public async Task Should_be_able_to_download_package_from_external_bot()
        {
            var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667, "client" + RandomString());
            await animeXdccClient.DownloadPackage("CR-HOLLAND|NEW", 5054);
        }

        private static string RandomString()
        {
            var random = new Random();
            return new string(
                Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 5)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
        }
    }
}