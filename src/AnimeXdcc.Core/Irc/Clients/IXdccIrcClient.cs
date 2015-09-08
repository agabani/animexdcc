using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Irc.Clients
{
    public interface IXdccIrcClient : IDisposable
    {
        Task<XdccIrcClient.IrcResult> RequestPackageAsync(string botName, int packageId, CancellationToken token = default(CancellationToken));
    }
}