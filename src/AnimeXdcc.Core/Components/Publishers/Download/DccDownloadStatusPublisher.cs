using System;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.SystemWrappers;

namespace AnimeXdcc.Core.Components.Publishers.Download
{
    public class DccDownloadStatusPublisher : IDownloadStatusPublisher
    {
        private readonly long _fileSize;
        private readonly long _resumePosition;
        private readonly ITimer _timer;
        private long _elapsedEvents;
        private long _transferredBytes;

        public DccDownloadStatusPublisher(ITimer timer, long fileSize, long resumePosition)
        {
            _timer = timer;
            _resumePosition = resumePosition;
            _fileSize = fileSize;
            _timer.Elapsed += TimerOnElapsed;
        }

        public event EventHandler<DccTransferStatus> TransferStatus;

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Update(long bytes)
        {
            _transferredBytes += bytes;
        }

        protected virtual void OnTransferStatus(DccTransferStatus e)
        {
            var handler = TransferStatus;
            if (handler != null) handler(this, e);
        }

        private void TimerOnElapsed(object sender, TimeElapsedEventArgs elapsedEventArgs)
        {
            _elapsedEvents ++;
            OnTransferStatus(CalculateStatus());
        }

        private DccTransferStatus CalculateStatus()
        {
            var downloadedBytes = _transferredBytes + _resumePosition;
            var elapsedTime = _timer.Interval*_elapsedEvents;
            var bytesPerMillisecond = _transferredBytes/elapsedTime;

            return new DccTransferStatus(_fileSize, elapsedTime, downloadedBytes, bytesPerMillisecond);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}