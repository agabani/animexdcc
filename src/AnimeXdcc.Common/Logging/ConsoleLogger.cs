using System;

namespace AnimeXdcc.Common.Logging
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
                Console.WriteLine(message);
            }
        }

        public void Info(string message)
        {
            if (_level >= Level.Info)
            {
                Console.WriteLine(message);
            }
        }

        public void Warn(string message)
        {
            if (_level >= Level.Warn)
            {
                Console.WriteLine(message);
            }
        }

        public void Error(string message)
        {
            if (_level >= Level.Error)
            {
                Console.WriteLine(message);
            }
        }

        public void Fatal(string message)
        {
            if (_level >= Level.Fatal)
            {
                Console.WriteLine(message);
            }
        }
    }
}