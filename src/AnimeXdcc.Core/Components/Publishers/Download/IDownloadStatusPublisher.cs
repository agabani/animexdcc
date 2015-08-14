using System;
using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Components.Publishers.Download
{
    public interface IDownloadStatusPublisher : IDisposable
    {
        event EventHandler<DccTransferStatus> TransferStatus;
        void Start();
        void Stop();
        void Update(long bytes);
    }
}