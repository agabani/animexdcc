using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Generic.DccClient.Models;

namespace Generic.DccClient.Publishers
{
    public class TransferStatusPublisher2
    {
        private readonly ConcurrentQueue<long> _bytesHistory = new ConcurrentQueue<long>();
        private readonly int _id;
        private readonly Action<TransferStatus> _publish;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Timer _timer = new Timer(2000);
        private readonly long _totalBytes;
        private long _transferredBytes;

        public TransferStatusPublisher2(Action<TransferStatus> publish, int id, long totalBytes)
        {
            _publish = publish;
            _id = id;
            _totalBytes = totalBytes;
            _timer.Elapsed += TimerOnElapsed;
            _stopwatch.Start();
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _bytesHistory.Enqueue(_transferredBytes);

            while (_bytesHistory.Count > 5)
            {
                long lon;
                _bytesHistory.TryDequeue(out lon);
            }

            var lowBytes = _bytesHistory.Count > 1 ? _bytesHistory.First() : 0;
            var highBytes = _bytesHistory.Last();

            var averageBytesPerMillisecond = (highBytes - lowBytes)/(_bytesHistory.Count*2000);

            var remainingMilliseconds = averageBytesPerMillisecond > 0 ? (_totalBytes - _transferredBytes) / averageBytesPerMillisecond : 0;

            _publish(new TransferStatus
            {
                ElapsedTimeMilliseconds = _stopwatch.ElapsedMilliseconds,
                RemainingTimeMilliseconds = remainingMilliseconds,
                TotalBytes = _totalBytes,
                TransferedBytes = _transferredBytes,
                TransferId = _id,
                TransferSpeedBytesPerMillisecond = averageBytesPerMillisecond
            });
        }

        public void Publish(long bytes)
        {
            _transferredBytes = bytes;
        }
    }
}