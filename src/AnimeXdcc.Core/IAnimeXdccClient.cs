using System;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core
{
    public interface IAnimeXdccClient : IDisposable
    {
        Task<DccTransferStatus> DownloadPackageAsync(string target, int packageId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}