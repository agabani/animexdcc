using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Generic.DccClient
{
    public class XdccDccClient
    {
        private static int _transferCounter;

        public void Download(string ipAddress, int port, uint filesize, string path)
        {
            var transferTag = string.Format("[TRANSFER {0}]: ", ++_transferCounter);

            Trace.TraceInformation(transferTag + "Attempting an active DCC RECV connection");
            Trace.TraceInformation(transferTag + "Contacting host {0} on port {1}", ipAddress, port);

            using (var tcpClient = new TcpClient(ipAddress, port))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    Trace.TraceInformation(transferTag + "Connected to {0}:{1}", ipAddress, port);

                    var totalBytes = 0;
                    var buffer = new byte[4096];

                    Trace.TraceInformation(transferTag + "Transferring data");

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

                    Trace.TraceInformation(transferTag + "Data transfer terminated");
                }

                Trace.TraceInformation(transferTag + "Transfer completed");
            }
        }

        public async Task DownloadAsync(string ipAddress, int port, uint filesize, string path)
        {
            var transferTag = string.Format("[TRANSFER {0}]: ", ++_transferCounter);

            Trace.TraceInformation(transferTag + "Attempting an active DCC RECV connection");
            Trace.TraceInformation(transferTag + "Contacting host {0} on port {1}", ipAddress, port);

            using (var tcpClient = new TcpClient(ipAddress, port))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    Trace.TraceInformation(transferTag + "Connected to {0}:{1}", ipAddress, port);

                    var totalBytes = 0;
                    var buffer = new byte[1024];

                    Trace.TraceInformation(transferTag + "Transferring data");

                    using (var fileStream = File.Create(path))
                    {
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

                    Trace.TraceInformation(transferTag + "Data transfer terminated");
                }

                Trace.TraceInformation(transferTag + "Transfer completed");
            }
        }
    }
}