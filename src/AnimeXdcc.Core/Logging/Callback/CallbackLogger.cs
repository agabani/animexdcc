using System;

namespace AnimeXdcc.Core.Logging.Callback
{
    public class CallbackLogger : ILogger
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

        public CallbackLogger(Level level)
        {
            _level = level;
        }

        public void Debug(string message)
        {
            if (_level >= Level.Debug)
            {
                OnLogEvent(new LogEventArgs(message));
            }
        }

        public void Info(string message)
        {
            if (_level >= Level.Info)
            {
                OnLogEvent(new LogEventArgs(message));
            }
        }

        public void Warn(string message)
        {
            if (_level >= Level.Warn)
            {
                OnLogEvent(new LogEventArgs(message));
            }
        }

        public void Error(string message)
        {
            if (_level >= Level.Error)
            {
                OnLogEvent(new LogEventArgs(message));
            }
        }

        public void Fatal(string message)
        {
            if (_level >= Level.Fatal)
            {
                OnLogEvent(new LogEventArgs(message));
            }
        }

        public event EventHandler<LogEventArgs> LogEvent;

        protected virtual void OnLogEvent(LogEventArgs e)
        {
            var handler = LogEvent;
            if (handler != null) handler(this, e);
        }
    }
}