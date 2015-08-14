using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Dcc.Models;
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
            var cancellationTokenSource = new CancellationTokenSource();

            System.Console.WriteLine("Enter search term...");

            var searchTerm = System.Console.ReadLine();

            var intelHttpClient = new IntelHttpClient(
                new Uri("http://intel.haruhichan.com/"),
                new ConsoleLogger(ConsoleLogger.Level.Debug));

            var file = (await intelHttpClient
                .SearchAsync(searchTerm, cancellationTokenSource.Token))
                .Files.OrderByDescending(r => r.Requested)
                .First();

            Display(file);

            var nickname = new RandomWord().GetString(10);

            System.Console.WriteLine("Using ALIAS: {0}", nickname);

            var animeXdccClient = new AnimeXdccClient("irc.rizon.net", 6667, nickname);

            var dccTransferStatus =
                await
                    animeXdccClient.DownloadPackageAsync(file.BotName, file.PackageNumber, cancellationTokenSource.Token);

            await Finish(dccTransferStatus);
        }

        private static void Display(File file)
        {
            System.Console.WriteLine("========================================\n" +
                                     "File name: {0}\nFile size: {1}\nBot name: {2}\nPackage Id: {3}\nRequested: {4}\n" +
                                     "========================================\n",
                file.FileName, file.Size, file.BotName, file.PackageNumber, file.Requested);
        }

        private static async Task Finish(DccTransferStatus dccTransferStatus)
        {
            Display(dccTransferStatus);

            var delayTask = Task.Delay(10000);

            var task = Task.Run(() =>
            {
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            });

            await Task.WhenAny(delayTask, task);
        }

        private static void Display(DccTransferStatus status)
        {
            System.Console.WriteLine("Download terminated:\n{0:N0}/{1:N0} [{2:N2}%] @ {3:N0} KB/s"
                , status.DownloadedBytes,
                status.FileSize,
                status.DownloadedBytes/(double) status.FileSize*100,
                status.BytesPerMillisecond);
        }
    }
}