using System;

namespace AnimeXdcc.Wpf.Models
{
    internal class DownloadProgress
    {
        public int PercentageComplete { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public long TransferSpeed { get; set; }
        public long TransferedBytes { get; set; }
        public long RemainingBytes { get; set; }
        public long TotalBytes { get; set; }
    }
}