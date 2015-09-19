using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Models;

namespace AnimeXdcc.Core.Services
{
    public interface ISearchService
    {
        Task<List<DccSearchResults>> SearchAsync(string searchTerm,
            CancellationToken token = default(CancellationToken));
    }
}