using System;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Logging.Console;
using Intel.Haruhichan.ApiClient.Clients;
using Intel.Haruhichan.ApiClient.Models;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class FullStackTests
    {
        [Test]
        public async Task Download_One_Piece_703()
        {
            var intelHttpClient = new IntelHttpClient(
                new Uri("http://intel.haruhichan.com/"),
                new ConsoleLogger(ConsoleLogger.Level.Debug));

            var file = (await intelHttpClient.SearchAsync("One Piece 703 1080p"))
                .Files
                .OrderByDescending(r => r.Requested)
                .First();

            Display(file);

            using (var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667, "speechlessdown"))
            {
                await animeXdccClient.DownloadPackageAsync(file.BotName, file.PackageNumber);
            }
        }

        private static void Display(File file)
        {
            Console.WriteLine("File name: {0}\nFile size: {1}\nBot name: {2}\nPackage Id: {3}\nRequested: {4}",
                file.FileName, file.Size, file.BotName, file.PackageNumber, file.Requested);
        }
    }
}