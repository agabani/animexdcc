using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Integration.Clients
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
                await CopyStreamAsync(stream, networkStream, stream.Length);
            }
        }

        private static async Task CopyStreamAsync(Stream input, NetworkStream output, long length)
        {
            var writeStreamTask = WriteStream(input, output);
            var readConfirmationTask = ReadConfirmation(output, length);
            await writeStreamTask;
            await readConfirmationTask;
        }

        private static async Task WriteStream(Stream input, NetworkStream output)
        {
            var buffer = new byte[1024];

            int bytes;
            do
            {
                bytes = await input.ReadAsync(buffer, 0, buffer.Length);
                await output.WriteAsync(buffer, 0, bytes);
            } while (bytes > 0);
        }

        private static async Task ReadConfirmation(NetworkStream output, long length)
        {
            long dataReceived;
            int bytes;

            var buffer = new byte[4];

            do
            {
                bytes = await output.ReadAsync(buffer, 0, buffer.Length);
                dataReceived = ToInt32(buffer);
            } while (dataReceived < length && bytes > 0);

            if (dataReceived < length)
            {
                throw new Exception(string.Format("Broken Pipe: {0}/{1} bytes transfered", dataReceived, length));
            }
        }

        private static int ToInt32(byte[] buffer)
        {
            return BitConverter.ToInt32(ReverseEndian(buffer), 0);
        }

        public static byte[] ReverseEndian(byte[] array)
        {
            return new[] {array[3], array[2], array[1], array[0]};
        }
    }
}