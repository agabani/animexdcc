using System.Timers;

namespace AnimeXdcc.Core.SystemWrappers
{
    public interface ITimer
    {
        void Start();
        void Stop();
        event ElapsedEventHandler Elapsed;
        double Interval { get; }
    }
}