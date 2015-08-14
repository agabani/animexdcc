using System;

namespace AnimeXdcc.Core.SystemWrappers
{
    public class TimeElapsedEventArgs : EventArgs
    {
        public DateTime SignalTime { get; private set; }

        public TimeElapsedEventArgs() : this(DateTime.Now)
        {
        }

        public TimeElapsedEventArgs(DateTime signalTime)
        {
            SignalTime = signalTime;
        }
    }
}