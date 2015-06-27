using System.Diagnostics;

namespace AnimeXdcc.Common.Logging
{
    public class TraceLogger : ILogger
    {
        public enum Level
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        private readonly Level _level;

        public TraceLogger(Level level)
        {
            _level = level;
        }

        public void Debug(string message)
        {
            if (_level >= Level.Debug)
            {
                Trace.TraceInformation(message);
            }
        }

        public void Info(string message)
        {
            if (_level >= Level.Info)
            {
                Trace.TraceInformation(message);
            }
        }

        public void Warn(string message)
        {
            if (_level >= Level.Warn)
            {
                Trace.TraceWarning(message);
            }
        }

        public void Error(string message)
        {
            if (_level >= Level.Error)
            {
                Trace.TraceError(message);
            }
        }

        public void Fatal(string message)
        {
            if (_level >= Level.Fatal)
            {
                Trace.TraceError(message);
            }
        }
    }
}