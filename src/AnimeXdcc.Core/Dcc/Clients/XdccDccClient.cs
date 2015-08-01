using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;
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

        public XdccDccClient()
        {
            _timer = new Timer(250);
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

                        var writeAsyncTask = WriteAsync(stream, buffer, bytesReceived);
                        await SendAcknowledgementAsync(networkStream, _transferredBytes += bytesReceived);
                        await writeAsyncTask;
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

        private static long Seek(Stream stream, long resumePosition)
        {
            return stream.Seek(resumePosition, SeekOrigin.Begin);
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