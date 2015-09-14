using System;

namespace AnimeXdcc.Core.Clients.Dcc.Models
{
    public class DccTransferStatus : EventArgs
    {
        public DccTransferStatus(long fileSize, double elapsedTime, long downloadedBytes, double bytesPerMillisecond)
        {
            FileSize = fileSize;
            ElapsedTime = elapsedTime;
            DownloadedBytes = downloadedBytes;
            BytesPerMillisecond = bytesPerMillisecond;
        }

        public long DownloadedBytes { get; private set; }
        public long FileSize { get; private set; }
        public double ElapsedTime { get; private set; }
        public double BytesPerMillisecond { get; private set; }
    }
}