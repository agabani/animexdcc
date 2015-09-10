using AnimeXdcc.Core.SystemWrappers.Stopwatch;
using AnimeXdcc.Core.SystemWrappers.Timer;

namespace AnimeXdcc.Core.Dcc.Components
{
    public class DccClientFactory : IDccClientFactory
    {
        private int _interval;

        public DccClientFactory(int interval)
        {
            _interval = interval;
        }

        public IDccClient Create(long fileSize)
        {
            _interval = 1000;
            return new DccClient(
                new TimerWrapper(_interval),
                new StopwatchWrapper(),
                new DccTransferFactory(),
                new DccTransferStatistics(fileSize, _interval));
        }
    }
}