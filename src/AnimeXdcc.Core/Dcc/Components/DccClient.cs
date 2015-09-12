using System;
using System.IO;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.SystemWrappers.Stopwatch;
using AnimeXdcc.Core.SystemWrappers.Timer;

namespace AnimeXdcc.Core.Dcc.Components
{
    public class DccClient : IDccClient
    {
        public delegate void TransferProgressEventHandler(DccClient sender, DccClientTransferProgressEventArgs args);

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

        private void DccTransferOnTransferBegun(DccTransfer sender, EventArgs args)
        {
            _stopwatch.Start();
            _timer.Start();
        }

        private void DccTransferOnTransferFailed(DccTransfer sender, EventArgs args)
        {
            _stopwatch.Stop();
            _timer.Stop();
        }

        private void DccTransferOnTransferComplete(DccTransfer sender, EventArgs args)
        {
            _stopwatch.Stop();
            _timer.Stop();
        }

        private void DccTransferOnTransferProgress(DccTransfer sender, DccTransferProgressEventArgs args)
        {
            _bytesTransferred = args.Transferred;
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