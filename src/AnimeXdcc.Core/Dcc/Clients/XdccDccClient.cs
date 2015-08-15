using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Publishers.Download;
using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Dcc.Clients
{
    public class XdccDccClient : IXdccDccClient
    {
        private readonly Stopwatch _stopwatch;
        private IDownloadStatusPublisher _publisher;
        private long _resumePosition;
        private long _transferredBytes;

        public XdccDccClient(IDownloadStatusPublisher publisher)
        {
            _publisher = publisher;
            _publisher.TransferStatus += PublisherOnTransferStatus;
            _stopwatch = new Stopwatch();
        }

        private long DownloadedBytes
        {
            get { return _resumePosition + _transferredBytes; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<DccTransferStatus> TransferStatus;

        public async Task<DccTransferStatus> DownloadAsync(Stream stream, IPAddress ipAddress, int port, long fileSize,
            long resumePosition, CancellationToken cancellationToken = default (CancellationToken))
        {
            _publisher.Setup(fileSize, resumePosition);

            Seek(stream, resumePosition);

            _transferredBytes = 0;
            _resumePosition = resumePosition;

            using (var tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(ipAddress, port);

                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[1024];
                    int bytesReceived;

                    _publisher.Start();
                    _stopwatch.Restart();

                    do
                    {
                        bytesReceived = await ReadAsync(networkStream, buffer, cancellationToken);

                        if (cancellationToken.IsCancellationRequested)
                        {
                            continue;
                        }

                        await WriteAsync(stream, buffer, bytesReceived, cancellationToken);

                        SendAcknowledgementAsync(networkStream, _transferredBytes += bytesReceived, cancellationToken)
                            .ConfigureAwait(false);

                        _publisher.Update(bytesReceived);
                    } while (
                        DataReceived(bytesReceived) &&
                        !TransferComplete(fileSize) &&
                        !cancellationToken.IsCancellationRequested
                        );

                    _stopwatch.Stop();
                    _publisher.Start();
                }
            }

            return DccTransferStatus(fileSize);
        }

        ~XdccDccClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_publisher != null)
                {
                    _publisher.Dispose();
                    _publisher = null;
                }
            }
        }

        private bool TransferComplete(long fileSize)
        {
            return DownloadedBytes >= fileSize;
        }

        private static bool DataReceived(int bytesReceived)
        {
            return bytesReceived > 0;
        }

        private DccTransferStatus DccTransferStatus(long fileSize)
        {
            return new DccTransferStatus(fileSize, _stopwatch.ElapsedMilliseconds, DownloadedBytes,
                _transferredBytes/(double) _stopwatch.ElapsedMilliseconds);
        }

        protected virtual void OnDccTransferStatus(DccTransferStatus e)
        {
            var handler = TransferStatus;
            if (handler != null) handler(this, e);
        }

        private void PublisherOnTransferStatus(object sender, DccTransferStatus dccTransferStatus)
        {
            OnDccTransferStatus(dccTransferStatus);
        }

        private static void Seek(Stream stream, long resumePosition)
        {
            stream.Seek(resumePosition, SeekOrigin.Begin);
        }

        private static Task<int> ReadAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
        {
            return stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        }

        private static Task WriteAsync(Stream stream, byte[] buffer, int bytes, CancellationToken cancellationToken)
        {
            return stream.WriteAsync(buffer, 0, bytes, cancellationToken);
        }

        private static Task SendAcknowledgementAsync(Stream stream, long transferred,
            CancellationToken cancellationToken)
        {
            var sendBuffer = StandardiseEndian(BitConverter.GetBytes(transferred));
            return stream.WriteAsync(sendBuffer, 0, sendBuffer.Length, cancellationToken);
        }

        private static byte[] StandardiseEndian(byte[] bytes)
        {
            return BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes;
        }
    }
}