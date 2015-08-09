using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core.Utilities;
using Integration.Clients;

namespace Integration.Bots
{
    public class SimpleXdccBot
    {
        private readonly IntegrationIrcClient _integrationIrcClient = new IntegrationIrcClient();
        private readonly string _hostname;
        private readonly int _port;
        private readonly string _nickname;
        private readonly string _filePath;

        public SimpleXdccBot(string nickname, string hostname, int port, string filePath)
        {
            _hostname = hostname;
            _port = port;
            _nickname = nickname;
            _filePath = filePath;
        }

        public async Task HostFile(string nickname)
        {
            var port = 12345 + new Random().Next(10);

            await Connect(_integrationIrcClient);
            await JoinChannel(_integrationIrcClient);
            await RecievePrivateMessage(_integrationIrcClient, nickname);

            var dccClient = new IntegrationDccClient();

            using (var file = OpenFileRead())
            {
                var dccSendTask = dccClient.Send(port, file, file.Length);
                await _integrationIrcClient.SendPrivateMessage(nickname, CreateDccSendMessage(file, port));
                await dccSendTask;
            }
        }

        private Task Connect(IntegrationIrcClient integrationIrcClient)
        {
            return integrationIrcClient.Connect(_hostname, _port, false, _nickname, null);
        }

        private static Task JoinChannel(IntegrationIrcClient integrationIrcClient)
        {
            return integrationIrcClient.Join(new[] {"#speechless"});
        }

        private static Task RecievePrivateMessage(IntegrationIrcClient integrationIrcClient, string nickname)
        {
            return integrationIrcClient.RecievePrivateMessage(nickname, "XDCC SEND #1");
        }

        private FileStream OpenFileRead()
        {
            return File.OpenRead(_filePath);
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