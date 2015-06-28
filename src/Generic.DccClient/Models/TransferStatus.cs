namespace Generic.DccClient.Models
{
    public class TransferStatus
    {
        public int TransferId { get; set; }
        public long TransferedBytes { get; set; }
        public long TotalBytes { get; set; }
        public long TransferSpeedBytesPerMillisecond { get; set; }
        public long ElapsedTimeMilliseconds { get; set; }
        public long RemainingTimeMilliseconds { get; set; }
    }
}