using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Models;

namespace AnimeXdcc.Core.Components.Searchable
{
    public interface ISearchable
    {
        Task<List<DccPackage>> SearchAsync(string searchTerm, CancellationToken token = default (CancellationToken));
    }
}