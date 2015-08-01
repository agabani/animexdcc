using System.Threading.Tasks;
using AnimeXdcc.Core.Irc.Clients;
using Integration.Clients;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class XdccIrcClientTest
    {
        [Test]
        public async Task Should_request_file()
        {
            const string integrationIrcClientNickname = "speechlessintegration";
            const string xdccIrcClientNickname = "speechlessdownloader";
            const string ircServerHostname = "irc.rizon.net";
            const int ircServerPort = 6667;
            const string ircChannel = "#speech";

            var integrationIrcClient = new IntegrationIrcClient();
            await integrationIrcClient.Connect(ircServerHostname, ircServerPort, false, integrationIrcClientNickname, null);
            await integrationIrcClient.Join(new[] {ircChannel});

            var watchJoin = integrationIrcClient.WatchJoin(new[] {ircChannel}, xdccIrcClientNickname);
            var recievePrivateMessage = integrationIrcClient.RecievePrivateMessage("\x01XDCC SEND #1\x01");

            var xdccIrcClient = new XdccIrcClient(ircServerHostname, ircServerPort);
            var requestPackage = xdccIrcClient.RequestPackageAsync(integrationIrcClientNickname, 1);

            await watchJoin;
            await recievePrivateMessage;

            await integrationIrcClient.SendPrivateMessage(xdccIrcClientNickname, "successful");

            var result = await requestPackage;

            Assert.That(result, Is.EqualTo("successful"));
        }
    }
}