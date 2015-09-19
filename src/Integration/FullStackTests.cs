using System;
using System.Collections.Generic;
using System.Linq;
using AnimeXdcc.Core.Clients;
using AnimeXdcc.Core.Clients.Dcc.Components;
using AnimeXdcc.Core.Clients.Irc.Components;
using AnimeXdcc.Core.Components.Converters;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Core.Components.Parsers.Dcc;
using AnimeXdcc.Core.Components.Searchable;
using AnimeXdcc.Core.Services;
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
                    new IntelSearchable(new IntelHttpClient(new Uri("http://intel.haruhichan.com/")))
                });

            var result = s.SearchAsync("One Piece 703 480p").GetAwaiter().GetResult();

            var x = new DownloadClient(
                new IrcClient("irc.rizon.net", 6667, "speechlessdown"),
                new DccClientFactory(1000),
                new DccMessageParser(new IpConverter()));

            x.DownloadAsync(result.First().DccPackages.First(), new StreamProvider(), null).GetAwaiter().GetResult();
        }
    }
}