using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Generic.DccClient.Models;
using Generic.DccClient.SystemWrappers;

namespace Generic.DccClient.Publishers
{
    public class TransferStatusPublisher2
    {
        private readonly Action<TransferStatus> _publish;
        private readonly int _id;
        private readonly long _totalBytes;
        private readonly ITimer _timer;
        private readonly IStopwatch _stopwatch;
        private long _transferedBytes;
        private readonly Queue<long> _history = new Queue<long>();

        public TransferStatusPublisher2(Action<TransferStatus> publish, int id, long totalBytes, ITimer timer, IStopwatch stopwatch)
        {
            _publish = publish;
            _id = id;
            _totalBytes = totalBytes;
            _timer = timer;
            _stopwatch = stopwatch;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            _stopwatch.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _history.Enqueue(_transferedBytes);

            while (_history.Count > 6)
            {
                _history.Dequeue();
            }

            long avg = 0;

            if (_history.Count > 1)
            {
                var min = _history.First();
                var max = _history.Last();

                var diff = (max - min);
                var time = (long)_timer.Interval * (_history.Count - 1);
                avg = diff / time;
            }

            long remainingTime = 0;

            if (avg > 0)
            {
                var remainingBytes = _totalBytes - _transferedBytes;
                remainingTime = remainingBytes/avg;
            }
           
            _publish(new TransferStatus
            {
                TransferId = _id,
                TotalBytes = _totalBytes,
                TransferedBytes = _transferedBytes,
                ElapsedTimeMilliseconds = _stopwatch.ElapsedMilliseconds,
                TransferSpeedBytesPerMillisecond = avg,
                RemainingTimeMilliseconds = remainingTime
            });
        }

        public void Publish(long bytes)
        {
            _transferedBytes += bytes;
        }
    }
}