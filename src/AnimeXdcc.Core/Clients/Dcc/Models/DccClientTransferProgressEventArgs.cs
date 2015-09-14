using System;

namespace AnimeXdcc.Core.Clients.Dcc.Models
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