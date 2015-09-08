using System;
using System.IO;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.SystemWrappers.Stopwatch;
using AnimeXdcc.Core.SystemWrappers.Timer;

namespace AnimeXdcc.Core.Dcc.Components
{
    public class DccClient
    {
        public enum DccFailureKind
        {
            None,
            RemoteHostRefusedConnection,
            RemoteHostClosedConnection,
            RemoteHostNotReachable
        }

        private readonly IDccTransferFactory _factory;
        private readonly IStopwatch _stopwatch;
        private readonly ITimer _timer;

        public DccClient(ITimer timer, IStopwatch stopwatch, IDccTransferFactory factory)
        {
            _timer = timer;
            _stopwatch = stopwatch;
            _factory = factory;
        }

        public async Task DownloadAsync(string hostname, int port, long size, Stream stream)
        {
            var dccTransfer = _factory.Create(hostname, port);

            dccTransfer.TransferBegun += DccTransferOnTransferBegun;
            dccTransfer.TransferFailed += DccTransferOnTransferFailed;
            dccTransfer.TransferComplete += DccTransferOnTransferComplete;
            dccTransfer.TransferProgress += DccTransferOnTransferProgress;

            await dccTransfer.Accept(stream, 0, size);
        }

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

    public class DccTransferStatistics
    {
        private readonly long _fileSize;
        private readonly long _interval;

        public DccTransferStatistics(long fileSize, long interval)
        {
            _fileSize = fileSize;
            _interval = interval;
        }

        public void AddDataSet(long transferred)
        {
        }

        public void GetStatistics()
        {
        }

        public class DccTransferStatistic
        {
            public long FileSize { get; private set; }
            public long BytesTransferred { get; private set; }
            public long BytesRemaining { get; private set; }
            public long SecondsElapsed { get; private set; }
            public long SecondsRemainging { get; private set; }
            public double BytesPerSecond { get; private set; }
            public double PercentageComplete { get; private set; }
        }
    }
}