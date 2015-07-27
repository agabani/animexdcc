using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Clients;
using AnimeXdcc.Core.Irc;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Clients;

namespace AnimeXdcc.Client
{
    public class AnimeXdccClient
    {
        private const string LogTag = "[AnimeXdccClient] ";
        private readonly string _baseUrl;
        private readonly string _ircChannel;
        private readonly string _ircHostName;
        private readonly string _ircNickName;
        private readonly int _ircPort;
        private readonly string _ircRealName;
        private readonly string _ircUserName;
        private readonly ILogger _logger;
        private readonly string _searchTerm;

        public AnimeXdccClient(string searchTerm, string baseUrl, string ircHostName, int ircPort, string ircUserName,
            string ircRealName, string ircNickName, string ircChannel, ILogger logger)
        {
            _searchTerm = searchTerm;
            _baseUrl = baseUrl;
            _ircHostName = ircHostName;
            _ircPort = ircPort;
            _ircUserName = ircUserName;
            _ircRealName = ircRealName;
            _ircNickName = ircNickName;
            _ircChannel = ircChannel;
            _logger = logger;
        }

        public async Task Run()
        {
            var searchString = _searchTerm;

            _logger.Info(LogTag + string.Format("[SEARCH] {0}", searchString));

            var intelHttpClient = new IntelHttpClient(new Uri(_baseUrl), _logger);
            var search = await intelHttpClient.Search(searchString);

            _logger.Info(LogTag + string.Format("[RESULT] {0} matches found", search.Files.Count()));

            if (search.Error)
            {
                return;
            }

            var package = search.Files.Aggregate((p1, p2) => p1.Requested > p2.Requested ? p1 : p2);

            _logger.Info(LogTag +
                         string.Format("[SELECTED] {0} from {1}. Package Id: {2}. Size: {3}. Requested: {4}.",
                             package.FileName.Replace("\r", string.Empty), package.BotName, package.PackageNumber,
                             package.Size, package.Requested));

            using (
                var xdccIrcClient = new XdccIrcClient(_ircNickName, _ircRealName, _ircUserName, _ircHostName, _ircPort,
                    _ircChannel, _logger))
            {
                xdccIrcClient.DccSendReceived += async (sender, message) =>
                {
                    var xdccDccClient = new XdccDccClient(_logger);

                    xdccDccClient.DccTransferredPacket +=
                        (o, status) =>
                        {
                            _logger.Info(LogTag +
                                         string.Format("[TRANSFER {0}] {1}/{2} @ {3}KB/s. ETA: {4} seconds.",
                                             status.TransferId,
                                             status.TransferedBytes, status.TotalBytes,
                                             status.TransferSpeedBytesPerMillisecond,
                                             status.RemainingTimeMilliseconds/1000));
                        };

                    await
                        xdccDccClient.DownloadAsync(message.IpAddress, message.Port, message.FileSize, message.FileName);
                    Environment.Exit(0);
                };

                xdccIrcClient.Run();

                Thread.Sleep(3000);

                xdccIrcClient.RequestPackage(package.BotName, package.PackageNumber);
            }

            while (true)
            {
                Thread.Sleep(1);
            }
        }
    }
}