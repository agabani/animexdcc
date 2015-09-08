using System;

namespace AnimeXdcc.Core.Dcc.Models
{
    public class DccTransferProgressEventArgs : EventArgs
    {
        public DccTransferProgressEventArgs(long transferred, long size)
        {
            Transferred = transferred;
            Size = size;
        }

        public long Transferred { get; private set; }
        public long Size { get; private set; }
    }
}