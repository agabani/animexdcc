using System.Timers;

namespace Generic.DccClient.SystemWrappers
{
    public interface ITimer
    {
        void Start();
        void Stop();
        event ElapsedEventHandler Elapsed;
        double Interval { get; }
    }
}