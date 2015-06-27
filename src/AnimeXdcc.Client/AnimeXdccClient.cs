using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Generic.DccClient;
using Generic.IrcClient;
using Intel.Haruhichan.ApiClient.Client;

namespace AnimeXdcc.Client
{
    public class AnimeXdccClient
    {
        private readonly string _searchTerm;
        private readonly string _baseUrl;
        private readonly string _ircHostName;
        private readonly int _ircPort;
        private readonly string _ircUserName;
        private readonly string _ircRealName;
        private readonly string _ircNickName;
        private readonly string _ircChannel;

        public AnimeXdccClient(string searchTerm, string baseUrl, string ircHostName, int ircPort, string ircUserName, string ircRealName, string ircNickName, string ircChannel)
        {
            _searchTerm = searchTerm;
            _baseUrl = baseUrl;
            _ircHostName = ircHostName;
            _ircPort = ircPort;
            _ircUserName = ircUserName;
            _ircRealName = ircRealName;
            _ircNickName = ircNickName;
            _ircChannel = ircChannel;
        }

        public async Task Run()
        {
            var searchString = _searchTerm;

            Console.WriteLine("[SEARCH] {0}", searchString);

            var intelHttpClient = new IntelHttpClient(new Uri(_baseUrl));
            var search = await intelHttpClient.Search(searchString);

            Console.WriteLine("[RESULT] {0} matches found", search.Files.Count());

            if (search.Error)
            {
                return;
            }

            var package = search.Files.Aggregate((p1, p2) => p1.Requested > p2.Requested ? p1 : p2);

            Console.WriteLine("[SELECTED] \"{0}\" from {1}. Package Id: {2}. Size: {3}. Requested: {4}.",
                package.FileName, package.BotName, package.PackageNumber, package.Size, package.Requested);

            var xdccIrcClient = new XdccIrcClient(
                _ircNickName,
                _ircRealName,
                _ircUserName,
                _ircHostName,
                _ircPort,
                _ircChannel);

            xdccIrcClient.DccSendReceived += (sender, message) =>
            {
                var xdccDccClient = new XdccDccClient();
                xdccDccClient.Download(message.IpAddress, message.Port, message.FileSize, message.FileName);
                Environment.Exit(0);
            };

            xdccIrcClient.Run();

            Thread.Sleep(3000);

            xdccIrcClient.RequestPackage(package.BotName, package.PackageNumber);

            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}
