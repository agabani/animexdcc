using System;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Clients;
using Intel.Haruhichan.ApiClient.Models;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class ProgramTests
    {
        private readonly AnimeXdccClient _animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667, "speechlessdown");

        private readonly IntelHttpClient _intelHttpClient = new IntelHttpClient(
            new Uri("http://intel.haruhichan.com/"),
            new ConsoleLogger(ConsoleLogger.Level.Debug));

        [Test]
        public async Task DownloadTask()
        {
            var file = (await _intelHttpClient.Search("One Piece 703 1080p"))
                .Files
                .OrderByDescending(r => r.Requested)
                .First();

            Display(file);

            await _animeXdccClient.DownloadPackage(file.BotName, file.PackageNumber);
        }

        private static void Display(File file)
        {
            Console.WriteLine("File name: {0}\nFile size: {1}\nBot name: {2}\nPackage Id: {3}\nRequested: {4}",
                file.FileName, file.Size, file.BotName, file.PackageNumber, file.Requested);
        }
    }
}