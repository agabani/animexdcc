using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Clients.Irc.Components
{
    public interface IIrcClient : IDisposable
    {
        Task<IrcClient.IrcResult> RequestPackageAsync(string botName, int packageId, CancellationToken token = default(CancellationToken));
    }
}