using System;
using System.Linq;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Clients;
using Intel.Haruhichan.ApiClient.Models;

namespace AnimeXdcc.Console
{
    internal class Program
    {
        public static void Main()
        {
            Task.Run(async () => await MainAsync()).Wait();
        }

        private static async Task MainAsync()
        {
            System.Console.WriteLine("Enter search term...");

            var searchTerm = System.Console.ReadLine();

            var intelHttpClient = new IntelHttpClient(
                new Uri("http://intel.haruhichan.com/"),
                new ConsoleLogger(ConsoleLogger.Level.Debug));

            var file = (await intelHttpClient
                .Search(searchTerm))
                .Files.OrderByDescending(r => r.Requested)
                .First();

            Display(file);

            var nickname = new RandomWord().GetString(10);

            System.Console.WriteLine("Using ALIAS: {0}", nickname);

            var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667, nickname);

            await animeXdccClient.DownloadPackage(file.BotName, file.PackageNumber);
        }

        private static void Display(File file)
        {
            System.Console.WriteLine("========================================\n" +
                                     "File name: {0}\nFile size: {1}\nBot name: {2}\nPackage Id: {3}\nRequested: {4}\n" +
                                     "========================================\n",
                file.FileName, file.Size, file.BotName, file.PackageNumber, file.Requested);
        }
    }
}