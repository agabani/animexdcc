using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using AnimeXdcc.Common.Logging;

namespace Generic.DccClient
{
    public class XdccDccClient
    {
        private const string LogTag = "[XdccDccClient] ";
        private static int _transferCounter;
        private readonly ILogger _logger;

        public XdccDccClient(ILogger logger)
        {
            _logger = logger;
        }

        public void Download(string ipAddress, int port, uint filesize, string path)
        {
            var transferTag = string.Format("[TRANSFER {0}]: ", ++_transferCounter);

            _logger.Info(LogTag + transferTag + "Attempting an active DCC RECV connection");
            _logger.Info(LogTag + transferTag + string.Format("Contacting host {0} on port {1}", ipAddress, port));

            using (var tcpClient = new TcpClient(ipAddress, port))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    _logger.Info(LogTag + transferTag + string.Format("Connected to {0}:{1}", ipAddress, port));

                    var totalBytes = 0;
                    var buffer = new byte[4096];

                    _logger.Info(LogTag + transferTag + "Transferring data");

                    using (var fileStream = File.Create(path))
                    {
                        int bytes;
                        do
                        {
                            bytes = networkStream.Read(buffer, 0, buffer.Length);

                            if (bytes > 0)
                            {
                                fileStream.Write(buffer, 0, bytes);
                                totalBytes += bytes;
                            }

                            if (totalBytes == filesize)
                            {
                                break;
                            }
                        } while (bytes > 0);
                    }

                    _logger.Info(LogTag + transferTag + "Data transfer terminated");
                }

                _logger.Info(LogTag + transferTag + "Transfer completed");
            }
        }

        public async Task DownloadAsync(string ipAddress, int port, uint filesize, string path)
        {
            var transferTag = string.Format("[TRANSFER {0}]: ", ++_transferCounter);

            _logger.Info(LogTag + transferTag + "Attempting an active DCC RECV connection");
            _logger.Info(LogTag + transferTag + string.Format("Contacting host {0} on port {1}", ipAddress, port));

            using (var tcpClient = new TcpClient(ipAddress, port))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    _logger.Info(LogTag + transferTag + string.Format("Connected to {0}:{1}", ipAddress, port));

                    var totalBytes = 0;
                    var buffer = new byte[1024];

                    _logger.Info(LogTag + transferTag + "Transferring data");

                    using (var fileStream = File.Create(path))
                    {
                        _logger.Debug(LogTag + transferTag + string.Format("Create file: {0}", path));

                        int bytes;
                        do
                        {
                            bytes = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                            if (bytes > 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytes);
                                totalBytes += bytes;
                            }

                            if (totalBytes == filesize)
                            {
                                break;
                            }

                        } while (bytes > 0);
                    }

                    _logger.Info(LogTag + transferTag + "Data transfer terminated");
                }

                _logger.Info(LogTag + transferTag + "Transfer completed");
            }
        }
    }
}