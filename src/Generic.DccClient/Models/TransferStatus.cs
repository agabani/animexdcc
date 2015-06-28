namespace Generic.DccClient.Models
{
    public class TransferStatus
    {
        public uint TransferId { get; set; }
        public uint TransferedBytes { get; set; }
        public uint TotalBytes { get; set; }
        public uint TransferSpeed { get; set; }
    }
}