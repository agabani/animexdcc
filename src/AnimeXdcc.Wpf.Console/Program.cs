using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients;
using AnimeXdcc.Core.Clients.Dcc.Components;
using AnimeXdcc.Core.Clients.Dcc.Models;
using AnimeXdcc.Core.Clients.Irc.Components;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Components.Converters;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Components.Notifications;
using AnimeXdcc.Core.Components.Parsers.Dcc;
using AnimeXdcc.Core.Components.Searchable;
using AnimeXdcc.Core.Components.UserName;
using AnimeXdcc.Core.Services;
using Intel.Haruhichan.ApiClient.Clients;

namespace AnimeXdcc.Wpf.Console
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            var application = new Application(
                new SearchService(
                    new List<ISearchable>
                    {
                        new IntelSearchable(
                            new IntelHttpClient(new Uri("http://intel.haruhichan.com")))
                    }),
                new DownloadService(
                    new DownloadClient(
                        new IrcClient("irc.rizon.net", 6667, new UserNameGenerator().Create(10)),
                        new DccClientFactory(1000),
                        new DccMessageParser(new IpConverter())),
                    new StreamProvider()));

            Task.Run(async () => await application.RunAsync()).GetAwaiter().GetResult();

            System.Console.ReadLine();
        }
    }

    public class Application
    {
        private readonly ISearchService _searchService;
        private readonly IDownloadService _downloadService;
        private readonly NotificationListener<DccTransferStatistic> _notificationListener;

        public Application(ISearchService searchService, IDownloadService downloadService)
        {
            _searchService = searchService;
            _downloadService = downloadService;
            _notificationListener = new NotificationListener<DccTransferStatistic>(ExecuteAsync);
        }

        public async Task RunAsync()
        {
            var package = await GetPackageInfo();
            await DownloadPackage(package);
        }

        private async Task<DccPackage> GetPackageInfo()
        {
            var dccSearchResults = await _searchService.SearchAsync("One Piece 707 720");

            foreach (var dccSearchResult in dccSearchResults)
            {
                System.Console.WriteLine(dccSearchResult.FileName, dccSearchResult.FileSize);
                foreach (var dccPackage in dccSearchResult.DccPackages)
                {
                    System.Console.WriteLine(@"/msg {0} xdcc send #{1}", dccPackage.BotName, dccPackage.PackageId);
                }
            }

            return dccSearchResults.First().DccPackages.First();
        }

        public async Task DownloadPackage(DccPackage package)
        {
            await _downloadService.DownloadAsync(package, _notificationListener);
        }

        public Task ExecuteAsync(DccTransferStatistic statistic)
        {
            System.Console.WriteLine(@"{0}/{1} @ {2}", statistic.BytesTransferred, statistic.FileSize ,statistic.BytesPerSecond);

            return Task.FromResult<object>(null);
        }
    }
}