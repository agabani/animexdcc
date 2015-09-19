using AnimeXdcc.Core.Components.SystemWrappers.Stopwatch;
using AnimeXdcc.Core.Components.SystemWrappers.Timer;

namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public class DccClientFactory : IDccClientFactory
    {
        private readonly int _interval;

        public DccClientFactory(int interval)
        {
            _interval = interval;
        }

        public IDccClient Create(long fileSize)
        {
            return new DccClient(
                new TimerWrapper(_interval),
                new StopwatchWrapper(),
                new DccTransferFactory(),
                new DccTransferStatistics(fileSize, _interval));
        }
    }
}