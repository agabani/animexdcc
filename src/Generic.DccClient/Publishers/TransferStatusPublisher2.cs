using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Generic.DccClient.Models;
using Generic.DccClient.SystemWrappers;

namespace Generic.DccClient.Publishers
{
    public class TransferStatusPublisher2
    {
        private readonly Queue<long> _history = new Queue<long>();
        private readonly int _id;
        private readonly Action<TransferStatus> _publish;
        private readonly IStopwatch _stopwatch;
        private readonly ITimer _timer;
        private readonly long _totalBytes;
        private long _transferedBytes;
        private const int Samples = 6;

        public TransferStatusPublisher2(Action<TransferStatus> publish, int id, long totalBytes, ITimer timer,
            IStopwatch stopwatch)
        {
            _publish = publish;
            _id = id;
            _totalBytes = totalBytes;
            _timer = timer;
            _stopwatch = stopwatch;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            _stopwatch.Start();
            _history.Enqueue(0);
        }

        public void Publish(long bytes)
        {
            _transferedBytes += bytes;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _history.Enqueue(_transferedBytes);

            while (_history.Count > Samples)
            {
                _history.Dequeue();
            }

            var avg = CalculateAverageTransferSpeed();

            var remainingTime = CalculateRemainingTime(avg);

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

        private long CalculateAverageTransferSpeed()
        {
            if (_history.Count <= 1)
            {
                return 0;
            }

            var minimum = _history.First();
            var maximum = _history.Last();
            var difference = (maximum - minimum);
            var time = (long) _timer.Interval*(_history.Count - 1);
            var average = difference/time;
            return average;
        }

        private long CalculateRemainingTime(long bytesPerMillisecond)
        {
            if (bytesPerMillisecond <= 0)
            {
                return long.MaxValue;
            }

            var remainingBytes = _totalBytes - _transferedBytes;
            var remainingTime = remainingBytes/bytesPerMillisecond;
            return remainingTime;
        }
    }
}