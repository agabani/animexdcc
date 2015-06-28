using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AnimeXdcc.Common.Logging;
using Generic.DccClient.Models;

namespace Generic.DccClient.Clients
{
    public class XdccDccClient
    {
        private const string LogTag = "[XdccDccClient] ";
        private static uint _transferCounter;
        private readonly ILogger _logger;

        public XdccDccClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<uint> DownloadAsync(string ipAddress, int port, uint filesize, string path)
        {
            var transferId = ++_transferCounter;

            var transferTag = string.Format("[TRANSFER {0}]: ", transferId);

            _logger.Info(LogTag + transferTag + "Attempting an active DCC RECV connection");
            _logger.Info(LogTag + transferTag + string.Format("Contacting host {0} on port {1}", ipAddress, port));

            var tcpClient = new TcpClient(ipAddress, port);
            var networkStream = tcpClient.GetStream();

            _logger.Info(LogTag + transferTag + string.Format("Connected to {0}:{1}", ipAddress, port));
            _logger.Info(LogTag + transferTag + "Transferring data");

            var fileStream = File.Create(path);

            _logger.Debug(LogTag + transferTag + string.Format("Create file: {0}", path));

            var remainingBytes = await CopyStreamAsync(networkStream, fileStream, filesize, _transferCounter);

            _logger.Info(LogTag + transferTag + "Data transfer terminated");
            _logger.Info(LogTag + transferTag + "Transfer completed");

            return remainingBytes;
        }

        private async Task<uint> CopyStreamAsync(Stream input, Stream output, uint bytes, uint id)
        {
            uint transferredBytes = 0;
            var buffer = new byte[8192];

            while (transferredBytes < bytes)
            {
                var readBytes = await input.ReadAsync(buffer, 0, buffer.Length);

                if (readBytes <= 0)
                {
                    break;
                }

                await output.WriteAsync(buffer, 0, readBytes);
                transferredBytes += (uint) readBytes;

                OnDccTransferredPacket(new TransferStatus
                {
                    TransferId = id,
                    TransferedBytes = transferredBytes,
                    TotalBytes = bytes,
                    TransferSpeed = 0
                });
            }

            return bytes - transferredBytes;
        }

        public event EventHandler<TransferStatus> DccTransferredPacket;

        protected virtual void OnDccTransferredPacket(TransferStatus e)
        {
            var handler = DccTransferredPacket;
            if (handler != null) handler(this, e);
        }
    }
}