using System;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.SystemWrappers.Timer;

namespace AnimeXdcc.Core.Components.Publishers.Download
{
    public class DccDownloadStatusPublisher : IDownloadStatusPublisher
    {
        private long _elapsedEvents;
        private long _fileSize;
        private long _resumePosition;
        private ITimer _timer;
        private long _transferredBytes;

        public DccDownloadStatusPublisher(ITimer timer)
        {
            _timer = timer;
            _timer.Elapsed += TimerOnElapsed;
        }

        public event EventHandler<DccTransferStatus> TransferStatus;

        public void Setup(long fileSize, long resumePosition)
        {
            _fileSize = fileSize;
            _resumePosition = resumePosition;
        }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DccDownloadStatusPublisher()
        {
            Dispose(false);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
    }
}