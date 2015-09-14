using AnimeXdcc.Core.Clients.Dcc.Models;

namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public class DccTransferStatistics : IDccTransferStatistics
    {
        private readonly long _fileSize;
        private readonly long _interval;
        private long _bytesTransferred;
        private double _datasets;

        public DccTransferStatistics(long fileSize, long interval)
        {
            _fileSize = fileSize;
            _interval = interval;
            _datasets = 0;
        }

        public void AddDataSet(long transferred)
        {
            _bytesTransferred = transferred;
            _datasets++;
        }

        public void FinalDataSet(long transferred, long elapsedMilliseconds)
        {
            _bytesTransferred = transferred;
            _datasets += (elapsedMilliseconds - _interval) / (double)_interval;
        }

        public DccTransferStatistic GetStatistics()
        {
            var secondsElapsed = _interval / 1000 * (long)_datasets;
            var bytesRemaining = _fileSize - _bytesTransferred;

            return new DccTransferStatistic(
                _fileSize, 
                _bytesTransferred, 
                bytesRemaining,
                secondsElapsed, 
                bytesRemaining/(_bytesTransferred/secondsElapsed),
                _bytesTransferred/(double) secondsElapsed, 
                _bytesTransferred*100/(double) _fileSize
                );
        }
    }
}