namespace AnimeXdcc.Core.Dcc.Models
{
    public class DccTransferStatistic
    {
        public DccTransferStatistic(
            long fileSize,
            long bytesTransferred,
            long bytesRemaining,
            long secondsElapsed,
            long secondsRemaining,
            double bytesPerSecond,
            double percentageComplete)
        {
            FileSize = fileSize;
            BytesTransferred = bytesTransferred;
            BytesRemaining = bytesRemaining;
            SecondsElapsed = secondsElapsed;
            SecondsRemaining = secondsRemaining;
            BytesPerSecond = bytesPerSecond;
            PercentageComplete = percentageComplete;
        }

        public long FileSize { get; private set; }
        public long BytesTransferred { get; private set; }
        public long BytesRemaining { get; private set; }
        public long SecondsElapsed { get; private set; }
        public long SecondsRemaining { get; private set; }
        public double BytesPerSecond { get; private set; }
        public double PercentageComplete { get; private set; }
    }
}