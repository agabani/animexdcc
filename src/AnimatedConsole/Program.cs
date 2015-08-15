using System;
using System.Threading.Tasks;
using Intel.Haruhichan.ApiClient.Models;

namespace AnimatedConsole
{
    internal class Program
    {
        private static void Main()
        {
            var consoleRenderer = new ConsoleRenderer();

            consoleRenderer.RenderTitle("AnimeXdcc v0.0.5");
            consoleRenderer.RenderInstructions();
            //string value = Console.ReadLine();
            Console.WriteLine("DanMachi HorribleSubs 1080p 01");

            File file = new File
            {
                BotId = 21,
                BotName = "Ginpachi-Sensei",
                FileName = "[HorribleSubs] DanMachi - 01 [1080p].mkv",
                PackageNumber = 54,
                Requested = 15,
                Size = "541M"
            };
            consoleRenderer.RenderFile(file.FileName, file.Size);

            consoleRenderer.DrawProgressBar();
            consoleRenderer.RenderCancel();

            for (var percent = 0; percent <= 100; percent++)
            {
                const int millisecondsDelay = 300;
                consoleRenderer.UpdateProgressBar(percent, 567976036, 9000*1024, millisecondsDelay/10 - millisecondsDelay/10*percent/100);
                Task.Delay(millisecondsDelay).Wait();
            }

            consoleRenderer.RenderDownloadComplete();
            consoleRenderer.RenderExit();
            Console.ReadKey();
        }
    }
}