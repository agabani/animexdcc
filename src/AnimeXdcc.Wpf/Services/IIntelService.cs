using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimeXdcc.Wpf.Services
{
    internal interface IIntelService
    {
        Task<List<Package>> SearchAsync(string searchTerm);
    }
}