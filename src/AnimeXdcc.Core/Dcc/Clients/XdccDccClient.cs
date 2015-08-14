using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
using AnimeXdcc.Core.Components.Publishers.Download;
using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Dcc.Clients
{
    public class XdccDccClient : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly Timer _timer;
        private long _elapsedEvents;
        private long _fileSize;
        private long _resumePosition;
        private long _transferredBytes;
        private readonly IDownloadStatusPublisher _publisher;

        public XdccDccClient(IDownloadStatusPublisher publisher)
        {
            _publisher = publisher;
            _timer = new Timer(1000);
            _timer.Elapsed += TimerOnElapsed;
            _stopwatch = new Stopwatch();
        }

        private long DownloadedBytes
        {
            get { return _resumePosition + _transferredBytes; }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        public event EventHandler<DccTransferStatus> TransferStatus;

        public async Task<DccTransferStatus> DownloadAsync(Stream stream, IPAddress ipAddress, int port, long fileSize,
            long resumePosition)
        {
            _publisher.Setup(fileSize, resumePosition);

            Seek(stream, resumePosition);

            _transferredBytes = 0;
            _elapsedEvents = 0;
            _fileSize = fileSize;
            _resumePosition = resumePosition;

            using (var tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(ipAddress, port);

                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[1024];
                    int bytesReceived;

                    _timer.Start();
                    _stopwatch.Restart();

                    do
                    {
                        bytesReceived = await ReadAsync(networkStream, buffer);

                        await WriteAsync(stream, buffer, bytesReceived);
                        SendAcknowledgementAsync(networkStream, _transferredBytes += bytesReceived).ConfigureAwait(false);
                    } while (bytesReceived > 0 && DownloadedBytes < fileSize);

                    _stopwatch.Stop();
                    _timer.Stop();
                }
            }

            return new DccTransferStatus(fileSize, _stopwatch.ElapsedMilliseconds, DownloadedBytes,
                _transferredBytes/(double) _stopwatch.ElapsedMilliseconds);
        }

        protected virtual void OnDccTransferStatus(DccTransferStatus e)
        {
            var handler = TransferStatus;
            if (handler != null) handler(this, e);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _elapsedEvents++;
            OnDccTransferStatus(CalculateStatus());
        }

        private DccTransferStatus CalculateStatus()
        {
            var downloadedBytes = _transferredBytes + _resumePosition;
            var elapsedTime = _timer.Interval*_elapsedEvents;
            var bytesPerMillisecond = _transferredBytes / elapsedTime;

            return new DccTransferStatus(_fileSize, elapsedTime, downloadedBytes, bytesPerMillisecond);
        }

        private static void Seek(Stream stream, long resumePosition)
        {
            stream.Seek(resumePosition, SeekOrigin.Begin);
        }

        private static Task<int> ReadAsync(Stream stream, byte[] buffer)
        {
            return stream.ReadAsync(buffer, 0, buffer.Length);
        }

        private static Task WriteAsync(Stream stream, byte[] buffer, int bytes)
        {
            return stream.WriteAsync(buffer, 0, bytes);
        }

        private static Task SendAcknowledgementAsync(Stream stream, long transferred)
        {
            var sendBuffer = StandardiseEndian(BitConverter.GetBytes(transferred));
            return stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
        }

        private static byte[] StandardiseEndian(byte[] bytes)
        {
            return BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes;
        }
    }
}