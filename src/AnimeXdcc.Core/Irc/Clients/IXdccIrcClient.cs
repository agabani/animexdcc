using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Irc.Clients
{
    public interface IXdccIrcClient : IDisposable
    {
        Task<string> RequestPackageAsync(string target, int packageId, CancellationToken cancellationToken = default(CancellationToken));
    }
}