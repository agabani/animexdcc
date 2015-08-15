using System;

namespace AnimatedConsole
{
    public class ConsoleRenderer
    {
        private const int LeftMargin = 1;
        private const int TopMargin = 1;
        private const int Width = 78;
        private static readonly string[] SizeSuffixes = {"B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        public ConsoleRenderer()
        {
            Console.SetWindowSize(80, 25);
        }

        public void RenderTitle(string title)
        {
            var line = string.Empty;
            for (var i = 0; i < Width; i++)
            {
                line += "=";
            }

            SetCursorPosition(0, 0);
            Write(line);
            SetCursorPosition(0, 1);
            Write("=");
            SetCursorPosition(Width - 1, 1);
            Write("=");
            SetCursorPosition(0, 2);
            Write(line);
            SetCursorPosition(2, 1);
            Write(title);
        }

        public void RenderInstructions()
        {
            SetCursorPosition(2, 4);
            Write("Enter search term, e.g.:");
            SetCursorPosition(2, 5);
            Write("* HorribleSubs DanMachi 08 1080p");
            SetCursorPosition(2, 6);
            Write("* HorribleSubs Fate Stay Night Unlimited Blade Works 03 1080p");
            SetCursorPosition(2, 8);
            Write("Search term: ");
        }

        public void RenderFile(string fileName, string fileSize)
        {
            SetCursorPosition(4, 10);
            Write(">> Best match:");
            SetCursorPosition(3, 11);
            Write(string.Format("{0} ({1})", fileName, fileSize));
        }

        public void RenderCancel()
        {
            const int top = 17;
            SetCursorPosition(3, top);
            Write("Press C to cancel download");
        }

        public void RenderExit()
        {
            const int top = 20;
            SetCursorPosition(2, top);
            Write("Press any key to exit...");
        }

        public static void RenderDownloadCancelled()
        {
            const int top = 17;
            SetCursorPosition(3, top);
            Write("Download has successfully cancelled.");
        }

        public void RenderDownloadComplete()
        {
            const int top = 17;
            SetCursorPosition(3, top);
            Write("Download has successfully completed.");
        }

        public void DrawProgressBar()
        {
            const int top = 15;

            SetCursorPosition(4, top);
            Write("[");
            SetCursorPosition(46, top);
            Write("]");
        }

        public void UpdateProgressBar(int percent, long size, long speed, double timeRemaining)
        {
            const int top = 15;

            SetCursorPosition(0, top);
            Write(string.Format("{0,3}%", percent));

            SetCursorPosition(5, top);
            for (var i = 0; i < (long) (40*percent/100); i++)
            {
                Write("=");
            }
            Write(">");

            SetCursorPosition(48, top);
            Write(SizeSuffix(size));

            SetCursorPosition(58, top);
            Write(string.Format("{0,20}", string.Format("{0}/s in {1:N1}s", SizeSuffix(speed), timeRemaining)));
        }

        private static void SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(LeftMargin + left, TopMargin + top);
        }

        private static void Write(string content)
        {
            Console.Write(content);
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