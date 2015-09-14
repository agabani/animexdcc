using System;

namespace AnimeXdcc.Core.Components.SystemWrappers.Timer
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