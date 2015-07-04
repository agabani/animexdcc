namespace Generic.DccClient.SystemWrappers
{
    public interface IStopwatch
    {
        long ElapsedMilliseconds { get; }
        void Start();
        void Stop();
        void Reset();
        void Restart();
    }
}