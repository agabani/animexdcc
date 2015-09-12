using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Dcc.Components
{
    public class DccTransferStatistics : IDccTransferStatistics
    {
        private readonly long _fileSize;
        private readonly long _interval;
        private long _bytesTransferred;
        private long _datasets;

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

        public DccTransferStatistic GetStatistics()
        {
            var secondsElapsed = _interval/1000*_datasets;
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