using System.Diagnostics;

namespace AnimeXdcc.Core.SystemWrappers
{
    public class StopwatchWrapper : IStopwatch
    {
        private readonly Stopwatch _stopwatch;

        public StopwatchWrapper()
        {
            _stopwatch = new Stopwatch();
        }

        public long ElapsedMilliseconds
        {
            get { return _stopwatch.ElapsedMilliseconds; }
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
        }

        public void Restart()
        {
            _stopwatch.Restart();
        }
    }
}