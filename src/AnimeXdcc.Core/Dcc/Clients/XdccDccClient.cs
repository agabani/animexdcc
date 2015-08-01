using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.Dcc.Publishers;
using AnimeXdcc.Core.Logging;
using AnimeXdcc.Core.SystemWrappers;

namespace AnimeXdcc.Core.Dcc.Clients
{
    [Obsolete]
    public class XdccDccClient
    {
        private const string LogTag = "[XdccDccClient] ";
        private static int _transferCounter;
        private readonly ILogger _logger;

        public XdccDccClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<long> DownloadAsync(string ipAddress, int port, long filesize, string path)
        {
            var transferId = NewTransferId();

            var transferTag = string.Format("[TRANSFER {0}]: ", transferId);

            _logger.Info(LogTag + transferTag + "Attempting an active DCC RECV connection");
            _logger.Info(LogTag + transferTag + string.Format("Contacting host {0} on port {1}", ipAddress, port));

            long remainingBytes;

            using (var tcpClient = new TcpClient(ipAddress, port))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    _logger.Info(LogTag + transferTag + string.Format("Connected to {0}:{1}", ipAddress, port));

                    using (var fileStream = File.Create(path))
                    {
                        _logger.Debug(LogTag + transferTag + string.Format("Create file: {0}", path));
                        _logger.Info(LogTag + transferTag + "Transferring data");

                        remainingBytes = await CopyStreamAsync(networkStream, fileStream, filesize, transferId);

                        _logger.Debug(LogTag + transferTag + string.Format("{0} bytes remaining", remainingBytes));
                    }

                    _logger.Info(LogTag + transferTag + "Data transfer terminated");
                }
            }

            _logger.Info(LogTag + transferTag + "Transfer completed");

            return remainingBytes;
        }

        private static int NewTransferId()
        {
            return ++_transferCounter;
        }

        private async Task<long> CopyStreamAsync(Stream input, Stream output, long bytes, int id)
        {
            long transferredBytes = 0;
            var buffer = new byte[8192];

            var transferStatusPublisher2 = new TransferStatusPublisher2(OnDccTransferredPacket, id, bytes,
                new TimerWrapper(2000), new StopwatchWrapper());

            while (transferredBytes < bytes)
            {
                var readBytes = await input.ReadAsync(buffer, 0, buffer.Length);

                if (readBytes <= 0)
                {
                    break;
                }

                await output.WriteAsync(buffer, 0, readBytes);
                transferredBytes += readBytes;

                await input.WriteAsync(ReverseEndian(BitConverter.GetBytes(transferredBytes)), 0, 4);

                transferStatusPublisher2.Publish(bytes);
            }

            return bytes - transferredBytes;
        }

        public event EventHandler<TransferStatus> DccTransferredPacket;

        protected virtual void OnDccTransferredPacket(TransferStatus e)
        {
            var handler = DccTransferredPacket;
            if (handler != null) handler(this, e);
        }

        private byte[] ReverseEndian(byte[] bytes)
        {
            return new[] {bytes[3], bytes[2], bytes[1], bytes[0]};
        }
    }
}