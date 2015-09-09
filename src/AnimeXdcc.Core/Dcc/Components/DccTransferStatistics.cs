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
            return new DccTransferStatistic(
                _fileSize, 
                _bytesTransferred, 
                _fileSize - _bytesTransferred,
                _interval*_datasets, 
                (_fileSize - _bytesTransferred)/(_bytesTransferred/(_interval*_datasets)),
                _bytesTransferred/(double) (_interval*_datasets), 
                _bytesTransferred*100/(double) _fileSize
                );
        }
    }
}