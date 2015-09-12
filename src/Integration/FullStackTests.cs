using System;
using System.Collections.Generic;
using System.Linq;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Dcc.Components;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Core.Logging.Trace;
using AnimeXdcc.Wpf.Services.Download;
using AnimeXdcc.Wpf.Services.Search;
using AnimeXdcc.Wpf.Services.Search.Searchable;
using Intel.Haruhichan.ApiClient.Clients;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class FullStackTests
    {
        [Test]
        public void Download_One_Piece_703()
        {
            var s =
                new SearchService(new List<ISearchable>
                {
                    new IntelSearchable(new IntelHttpClient(new Uri("http://intel.haruhichan.com/"),
                        new TraceLogger(TraceLogger.Level.Debug)))
                });

            var result = s.SearchAsync("One Piece 703 480p").GetAwaiter().GetResult();

            var x = new DownloadClient(new XdccIrcClient("irc.rizon.net", 6667, "speechlessdown"),
                new DccClientFactory(1000));

            x.DownloadAsync(result.First().DccPackages.First(), new StreamProvider(), null).GetAwaiter().GetResult();
        }
    }
}