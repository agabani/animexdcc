namespace AnimeXdcc.Core.Components.Logging.Console
{
    public class ConsoleLogger : ILogger
    {
        public enum Level
        {
            Fatal,
            Error,
            Warn,
            Info,
            Debug
        }

        private readonly Level _level;

        public ConsoleLogger(Level level)
        {
            _level = level;
        }

        public void Debug(string message)
        {
            if (_level >= Level.Debug)
            {
                System.Console.WriteLine(message);
            }
        }

        public void Info(string message)
        {
            if (_level >= Level.Info)
            {
                System.Console.WriteLine(message);
            }
        }

        public void Warn(string message)
        {
            if (_level >= Level.Warn)
            {
                System.Console.WriteLine(message);
            }
        }

        public void Error(string message)
        {
            if (_level >= Level.Error)
            {
                System.Console.WriteLine(message);
            }
        }

        public void Fatal(string message)
        {
            if (_level >= Level.Fatal)
            {
                System.Console.WriteLine(message);
            }
        }
    }
}