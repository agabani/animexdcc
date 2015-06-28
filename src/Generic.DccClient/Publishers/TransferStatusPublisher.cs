using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Generic.DccClient.Models;

namespace Generic.DccClient.Publishers
{
    public class TransferStatusPublisher
    {
        private readonly int _movingAverageResolution;
        private readonly Action<TransferStatus> _publish;
        private readonly int _publishRateMilliseconds;
        private readonly Stopwatch _stopwatch;
        private long _elapsedMilliseconds;
        private Queue<long> _transferHistory;

        public TransferStatusPublisher(Action<TransferStatus> publish)
        {
            _publish = publish;
            _stopwatch = new Stopwatch();
            _publishRateMilliseconds = 2000;
            _movingAverageResolution = 5;
        }

        public void NewSession()
        {
            _transferHistory = new Queue<long>();
            _stopwatch.Restart();
            _elapsedMilliseconds = 0;
        }

        public void Publish(int id, long bytes, long totalBytes)
        {
            if (_stopwatch.ElapsedMilliseconds >= _elapsedMilliseconds + _publishRateMilliseconds)
            {
                _elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
                _transferHistory.Enqueue(bytes);

                TrimMovingAverage();

                var averageBytesPerMillisecond = CalculateAverageSpeedBytesPerMilliseconds();

                var remainingMilliseconds = CalculateEstimatedTimeRemainingMilliseconds(bytes, totalBytes,
                    averageBytesPerMillisecond);

                _publish(new TransferStatus
                {
                    TransferSpeedBytesPerMillisecond = averageBytesPerMillisecond,
                    TotalBytes = totalBytes,
                    TransferedBytes = bytes,
                    TransferId = id,
                    ElapsedTimeMilliseconds = _elapsedMilliseconds,
                    RemainingTimeMilliseconds = remainingMilliseconds
                });
            }
        }

        private void TrimMovingAverage()
        {
            while (_transferHistory.Count > _movingAverageResolution)
            {
                _transferHistory.Dequeue();
            }
        }

        private long CalculateAverageSpeedBytesPerMilliseconds()
        {
            return (_transferHistory.Last() - (_transferHistory.Count > 1 ? _transferHistory.First() : 0))/
                   (_transferHistory.Count*_publishRateMilliseconds);
        }

        private static long CalculateEstimatedTimeRemainingMilliseconds(long bytes, long totalBytes,
            long averageBytesPerMillisecond)
        {
            return (totalBytes - bytes)/averageBytesPerMillisecond;
        }
    }
}