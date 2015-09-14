using AnimeXdcc.Core.Clients.Dcc.Models;

namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public interface IDccTransferStatistics
    {
        void AddDataSet(long transferred);
        DccTransferStatistic GetStatistics();
        void FinalDataSet(long transferred, long elapsedMilliseconds);
    }
}