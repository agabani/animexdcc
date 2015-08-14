using System.Threading;
using System.Threading.Tasks;
using Intel.Haruhichan.ApiClient.Models;

namespace Intel.Haruhichan.ApiClient.Clients
{
    public interface IIntelHttpClient
    {
        Task<Search> SearchAsync(string term, CancellationToken cancellationToken = default (CancellationToken));
        Task<Search> BotAsync(int id, CancellationToken cancellationToken = default (CancellationToken));
    }
}