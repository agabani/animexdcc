using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Generic.DccClient.Models;

namespace Generic.DccClient.Publishers
{
    public class TransferStatusPublisher
    {
        private readonly Action<TransferStatus> _publish;
        private readonly Stopwatch _stopwatch;
        private readonly Queue<uint> _transferHistory;
        private long _elapsedMilliseconds;

        public TransferStatusPublisher(Action<TransferStatus> publish)
        {
            _publish = publish;
            _stopwatch = new Stopwatch();
            _transferHistory = new Queue<uint>();
        }

        public void Publish(uint bytes, uint totalBytes, uint id)
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
            }

            if (_stopwatch.ElapsedMilliseconds >= _elapsedMilliseconds + 2000)
            {
                _elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
                _transferHistory.Enqueue(bytes);

                while (_transferHistory.Count > 5)
                {
                    _transferHistory.Dequeue();
                }

                var average = (_transferHistory.Last() - _transferHistory.First())/((uint) _transferHistory.Count*2);

                _publish(new TransferStatus
                {
                    TransferSpeed = average,
                    TotalBytes = totalBytes,
                    TransferedBytes = bytes,
                    TransferId = id
                });
            }
        }
    }
}