using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Dcc.Components
{
    public interface IDccTransferStatistics
    {
        void AddDataSet(long transferred);
        DccTransferStatistic GetStatistics();
    }
}