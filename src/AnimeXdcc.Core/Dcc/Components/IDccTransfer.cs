using System;
using System.IO;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Dcc.Components
{
    public interface IDccTransfer : IDisposable
    {
        Task AcceptAsync(Stream stream, long offset, long size);
        event DccTransfer.DccTransferEventHandler TransferBegun;
        event DccTransfer.DccTransferEventHandler TransferFailed;
        event DccTransfer.DccTransferEventHandler TransferComplete;
        event DccTransfer.DccTransferProgressEventHandler TransferProgress;
    }
}