using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Search
{
    public interface ISearchService
    {
        Task<List<DccSearchResults>> SearchAsync(string searchTerm,
            CancellationToken token = default(CancellationToken));
    }
}