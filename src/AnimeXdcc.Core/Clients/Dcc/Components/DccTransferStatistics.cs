using AnimeXdcc.Core.Clients.Dcc.Models;

namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public class DccTransferStatistics : IDccTransferStatistics
    {
        private readonly long _fileSize;
        private readonly long _intervalMilliseconds;
        private long _bytesTransferred;
        private double _datasets;

        public DccTransferStatistics(long fileSize, long intervalMilliseconds)
        {
            _fileSize = fileSize;
            _intervalMilliseconds = intervalMilliseconds;
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
            _datasets += (elapsedMilliseconds - _datasets*_intervalMilliseconds) / _intervalMilliseconds;
        }

        public DccTransferStatistic GetStatistics()
        {
            var secondsElapsed = _intervalMilliseconds / 1000 * (long)_datasets;
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