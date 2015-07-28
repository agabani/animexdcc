using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Integration
{
    public class DccClient
    {
        public async Task Send(int port, Stream stream, long size)
        {
            var tcpListener = new TcpListener(IPAddress.Any, port);

            tcpListener.Start();

            var tcpClient = await tcpListener.AcceptTcpClientAsync();

            using (var networkStream = tcpClient.GetStream())
            {
                await CopyStreamAsync(stream, networkStream);
            }
        }

        private static async Task CopyStreamAsync(Stream input, Stream output)
        {
            var buffer = new byte[1024];
            int bytes;

            do
            {
                bytes = await input.ReadAsync(buffer, 0, buffer.Length);
                await output.WriteAsync(buffer, 0, bytes);
            } while (bytes > 0);
        }
    }
}