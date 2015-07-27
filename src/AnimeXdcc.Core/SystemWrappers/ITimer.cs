using System.Timers;

namespace AnimeXdcc.Core.SystemWrappers
{
    public interface ITimer
    {
        double Interval { get; }
        void Start();
        void Stop();
        event ElapsedEventHandler Elapsed;
    }
}