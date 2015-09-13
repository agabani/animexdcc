using System;
using System.IO;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.SystemWrappers.Stopwatch;
using AnimeXdcc.Core.SystemWrappers.Timer;

namespace AnimeXdcc.Core.Dcc.Components
{
    // TODO: Make this entire class disposible as it is non renterant
    public class DccClient : IDccClient
    {
        public delegate void TransferProgressEventHandler(object sender, DccClientTransferProgressEventArgs e);

        public enum DccFailureKind
        {
            None,
            RemoteHostRefusedConnection,
            RemoteHostClosedConnection,
            RemoteHostNotReachable
        }

        private readonly IDccTransferFactory _transferFactory;
        private readonly IDccTransferStatistics _statistics;
        private readonly IStopwatch _stopwatch;
        private readonly ITimer _timer;
        private long _bytesTransferred;
        private long _size;

        public DccClient(ITimer timer, IStopwatch stopwatch, IDccTransferFactory transferFactory,
            IDccTransferStatistics statistics)
        {
            _timer = timer;
            _stopwatch = stopwatch;
            _transferFactory = transferFactory;
            _statistics = statistics;

            _timer.Elapsed += TimerOnElapsed;
        }

        public async Task DownloadAsync(string hostname, int port, long size, Stream stream)
        {
            _size = size;

            using (var dccTransfer = _transferFactory.Create(hostname, port))
            {
                dccTransfer.TransferBegun += DccTransferOnTransferBegun;
                dccTransfer.TransferFailed += DccTransferOnTransferFailed;
                dccTransfer.TransferComplete += DccTransferOnTransferComplete;
                dccTransfer.TransferProgress += DccTransferOnTransferProgress;

                await dccTransfer.AcceptAsync(stream, 0, size);
            }
        }

        public event TransferProgressEventHandler TransferProgress;

        private void DccTransferOnTransferBegun(object sender, EventArgs e)
        {
            _stopwatch.Start();
            _timer.Start();
        }

        private void DccTransferOnTransferFailed(object sender, EventArgs e)
        {
            _stopwatch.Stop();
            _timer.Stop();
        }

        private void DccTransferOnTransferComplete(object sender, EventArgs e)
        {
            _stopwatch.Stop();
            _statistics.FinalDataSet(_size, _stopwatch.ElapsedMilliseconds);
            OnTransferProgress(new DccClientTransferProgressEventArgs(_statistics.GetStatistics()));
            _timer.Stop();
        }

        private void DccTransferOnTransferProgress(object sender, DccTransferProgressEventArgs e)
        {
            _bytesTransferred = e.Transferred;
        }

        private void TimerOnElapsed(object sender, TimeElapsedEventArgs timeElapsedEventArgs)
        {
            _statistics.AddDataSet(_bytesTransferred);
            OnTransferProgress(new DccClientTransferProgressEventArgs(_statistics.GetStatistics()));
        }

        protected virtual void OnTransferProgress(DccClientTransferProgressEventArgs args)
        {
            var handler = TransferProgress;
            if (handler != null) handler(this, args);
        }

        public class DccResult
        {
            public DccResult(bool successful, DccFailureKind failure)
            {
                Successful = successful;
                Failure = failure;
            }

            public bool Successful { get; private set; }
            public DccFailureKind Failure { get; private set; }
        }
    }
}