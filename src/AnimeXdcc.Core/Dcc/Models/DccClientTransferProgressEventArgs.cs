using System;

namespace AnimeXdcc.Core.Dcc.Models
{
    public class DccClientTransferProgressEventArgs : EventArgs
    {
        public DccClientTransferProgressEventArgs(DccTransferStatistic statistic)
        {
            Statistic = statistic;
        }

        public DccTransferStatistic Statistic { get; private set; }
    }
}