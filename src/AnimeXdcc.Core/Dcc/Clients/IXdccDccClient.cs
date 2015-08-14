using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Dcc.Clients
{
    public interface IXdccDccClient : IDisposable
    {
        event EventHandler<DccTransferStatus> TransferStatus;

        Task<DccTransferStatus> DownloadAsync(Stream stream, IPAddress ipAddress, int port, long fileSize,
            long resumePosition, CancellationToken cancellationToken = default (CancellationToken));
    }
}