namespace AnimeXdcc.Core.Components.Logging.Trace
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
                System.Diagnostics.Trace.TraceInformation(message);
            }
        }

        public void Info(string message)
        {
            if (_level >= Level.Info)
            {
                System.Diagnostics.Trace.TraceInformation(message);
            }
        }

        public void Warn(string message)
        {
            if (_level >= Level.Warn)
            {
                System.Diagnostics.Trace.TraceWarning(message);
            }
        }

        public void Error(string message)
        {
            if (_level >= Level.Error)
            {
                System.Diagnostics.Trace.TraceError(message);
            }
        }

        public void Fatal(string message)
        {
            if (_level >= Level.Fatal)
            {
                System.Diagnostics.Trace.TraceError(message);
            }
        }
    }
}