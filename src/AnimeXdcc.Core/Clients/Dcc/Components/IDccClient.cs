using System;
using System.IO;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public interface IDccClient : IDisposable
    {
        Task<DccClient.DccResult> DownloadAsync(string hostname, int port, long size, Stream stream);
        event DccClient.TransferProgressEventHandler TransferProgress;
    }
}