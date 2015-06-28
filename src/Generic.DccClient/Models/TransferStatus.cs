namespace Generic.DccClient.Models
{
    public class TransferStatus
    {
        public int TransferId { get; set; }
        public long TransferedBytes { get; set; }
        public long TotalBytes { get; set; }
        public long TransferSpeed { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public long RemainingMilliseconds { get; set; }
    }
}