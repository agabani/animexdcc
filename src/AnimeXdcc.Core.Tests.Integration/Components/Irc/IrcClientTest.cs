using System;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Irc.Components;
using Integration.Clients;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Integration.Components.Irc
{
    [TestFixture]
    public class IrcClientTest
    {
        [Test]
        public async Task Should_be_able_to_request_a_xdcc_package()
        {
            var configration = new Configuration();
            var ircServer = configration.IrcServer.HostName;
            var port = configration.IrcServer.Port;

            var integrationNick = "server" + RandomString();
            var xdccIrcNick = "client" + RandomString();
            var ircChannel = "#" + RandomString();

            var integrationIrcClient = new IntegrationIrcClient();

            // -- Integration client connect to server
            Console.WriteLine("Integration connecting to {0}", ircServer);
            await integrationIrcClient.Connect(ircServer, port, false, integrationNick, null);
            await integrationIrcClient.Join(new[] {ircChannel});
            Console.WriteLine("Integration connected to {0}", ircServer);

            // -- Integration client wait for XDCC client to join channel
            var watchJoin = integrationIrcClient.WatchJoin(new[] { ircChannel }, xdccIrcNick);
            Console.WriteLine("Integration waiting for {0} to join {1}", xdccIrcNick, ircChannel);

            // -- Integration client wait for XDCC DCC message
            var recievePrivateMessage = integrationIrcClient.RecievePrivateMessage("XDCC SEND #1");
            Console.WriteLine("Integration waiting for {0} to send private message", xdccIrcNick);

            // -- XDCC client request package
            var ircClient = new IrcClient(ircServer, port, xdccIrcNick);
            Console.WriteLine("XDCC client requesting private message {0} with package id {1}", integrationNick, 1);
            var requestPackage = ircClient.RequestPackageAsync(integrationNick, 1);

            // -- Integration client confirm XDCC client joined channel
            await watchJoin;
            Console.WriteLine("Integration waited for {0} to join {1}", xdccIrcNick, ircChannel);

            // -- Integration client confirm XDCC DCC message
            await recievePrivateMessage;
            Console.WriteLine("Integration waited for {0} to send private message", xdccIrcNick);

            // -- Integration client send response
            var expected = RandomString();
            Console.WriteLine("Integration sending private message {0} to {1}", expected, xdccIrcNick);
            await integrationIrcClient.SendPrivateMessage(xdccIrcNick, expected);
            Console.WriteLine("Integration sent private message {0} to {1}", expected, xdccIrcNick);

            // -- XDCC client recieve result
            var actual = await requestPackage;
            Console.WriteLine("XDCC client recieved private message {0} from {1}", actual, integrationNick);

            Assert.That(actual.Result, Is.EqualTo(expected));
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