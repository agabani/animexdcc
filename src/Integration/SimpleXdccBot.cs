using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core.Irc;
using Integration.Clients;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class SimpleXdccBot
    {
        private readonly IrcClient _ircClient = new IrcClient();

        [Test]
        public async Task HostFile()
        {
            const string nickname = "speechless";
            var port = 12345 + new Random().Next(10);

            await Connect(_ircClient);
            await JoinChannel(_ircClient);
            await RecievePrivateMessage(_ircClient, nickname);

            var dccClient = new DccClient();

            using (var file = OpenFileRead())
            {
                var dccSendTask = dccClient.Send(port, file, file.Length);
                await _ircClient.SendPrivateMessage(nickname, CreateDccSendMessage(file, port));
                await dccSendTask;
            }
        }

        private static Task Connect(IrcClient ircClient)
        {
            return ircClient.Connect("irc.rizon.net", 6667, false, "ObserverXdccServer", null);
        }

        private static Task JoinChannel(IrcClient ircClient)
        {
            return ircClient.Join(new[] {"#speechless"});
        }

        private static Task RecievePrivateMessage(IrcClient ircClient, string nickname)
        {
            return ircClient.RecievePrivateMessage(nickname, "xdcc send #1");
        }

        private static FileStream OpenFileRead()
        {
            return File.OpenRead(@"Data\17 - Nintendo - Mute City Ver. 3.mp3");
        }

        private static string CreateDccSendMessage(FileStream file, int port)
        {
            return string.Format("\x01" + "DCC SEND {0} {1} {2} {3}" + "\x01", GetFileName(file),
                GetIpAddress("127.0.0.1"), port, file.Length);
        }

        private static long GetIpAddress(string ipAddress)
        {
            return new IpConverter().IpAddressToIntAddress(ipAddress);
        }

        private static string GetFileName(FileStream file)
        {
            return string.Format("\"{0}\"", file.Name.Split('\\').Last());
        }
    }
}