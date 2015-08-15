using System;
using System.Threading.Tasks;
using Intel.Haruhichan.ApiClient.Models;

namespace AnimatedConsole
{
    internal class Program
    {
        private static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        private static void Main()
        {
            RenderTitle();
            RenderInstructions();
            //string value = Console.ReadLine();
            Console.WriteLine("DanMachi HorribleSubs 1080p 01");

            RenderFile(new File
            {
                BotId = 21,
                BotName = "Ginpachi-Sensei",
                FileName = "[HorribleSubs] DanMachi - 01 [1080p].mkv",
                PackageNumber = 54,
                Requested = 15,
                Size = "541M"
            });

            DrawProgressBar();
            RenderCancel();

            for (var percent = 0; percent <= 100; percent++)
            {
                const int millisecondsDelay = 300;
                UpdateProgressBar(percent, 567976036, 9000*1024, millisecondsDelay/10 - millisecondsDelay/10*percent/100);
                Task.Delay(millisecondsDelay).Wait();
            }

            RenderDownloadComplete();
            RenderExit();
            Console.ReadKey();
        }

        public static void RenderTitle()
        {
            var width = GetConsoleWidth();

            var line = string.Empty;
            for (var i = 0; i < width; i++)
            {
                line += "=";
            }

            SetCursorPosition(0, 0);
            Console.Write(line);
            SetCursorPosition(0, 1);
            Console.Write("=");
            SetCursorPosition(width - 1, 1);
            Console.Write("=");
            SetCursorPosition(0, 2);
            Console.Write(line);
            SetCursorPosition(2, 1);
            Console.Write("AnimeXdcc v0.0.5");
        }

        public static void RenderInstructions()
        {
            SetCursorPosition(2, 4);
            Console.Write("Enter search term, e.g.:");
            SetCursorPosition(2, 5);
            Console.Write("* HorribleSubs DanMachi 08 1080p");
            SetCursorPosition(2, 6);
            Console.Write("* HorribleSubs Fate Stay Night Unlimited Blade Works 03 1080p");
            SetCursorPosition(2, 8);
            Console.Write("Search term: ");
        }

        public static void RenderFile(File file)
        {
            SetCursorPosition(4, 10);
            Console.Write(">> Best match:");
            SetCursorPosition(3, 11);
            Console.Write("{0} ({1})", file.FileName, file.Size);
        }

        public static void DrawProgressBar()
        {
            const int top = 15;

            SetCursorPosition(4, top);
            Console.Write("[");
            SetCursorPosition(46, top);
            Console.Write("]");
        }

        public static void RenderCancel()
        {
            const int top = 17;
            SetCursorPosition(3, top);
            Console.Write("Press C to cancel download");
        }

        public static void RenderExit()
        {
            const int top = 20;
            SetCursorPosition(2, top);
            Console.Write("Press any key to exit...");
        }

        public static void UpdateProgressBar(int percent, long size, long speed, double timeRemaining)
        {
            const int top = 15;

            SetCursorPosition(0, top);
            Console.Write("{0,3}%", percent);

            SetCursorPosition(5, top);
            for (var i = 0; i < (long) (40*percent/100); i++)
            {
                Console.Write("=");
            }
            Console.Write(">");

            SetCursorPosition(48, top);
            Console.Write(SizeSuffix(size));

            SetCursorPosition(58, top);
            Console.Write("{0,20}", string.Format("{0}/s in {1:N1}s", SizeSuffix(speed), timeRemaining));
        }

        public static void RenderDownloadCancelled()
        {
            const int top = 17;
            SetCursorPosition(3, top);
            Console.Write("Download has successfully cancelled.");
        }

        public static void RenderDownloadComplete()
        {
            const int top = 17;
            SetCursorPosition(3, top);
            Console.Write("Download has successfully completed.");
        }

        public static void SetCursorPosition(int left, int top)
        {
            const int leftMargin = 1;
            const int topMargin = 1;
            Console.SetCursorPosition(leftMargin + left, topMargin + top);
        }

        public static int GetConsoleWidth()
        {
            return Console.WindowWidth - 2;
        }

        private static string SizeSuffix(long value)
        {
            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }

            var i = 0;
            decimal dValue = value;
            while (Math.Floor(dValue/1024) >= 1)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:N1} {1}", dValue, SizeSuffixes[i]);
        }
    }
}