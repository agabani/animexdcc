using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using AnimeXdcc.Core.Dcc.Models;

namespace AnimeXdcc.Core.Dcc.Components
{
    public class DccTransfer : IDccTransfer
    {
        public delegate void DccTransferEventHandler(object sender, EventArgs e);

        public delegate void DccTransferProgressEventHandler(object sender, DccTransferProgressEventArgs e);

        private readonly byte[] _recvBuffer = new byte[1024];
        private TcpClient _client;
        private byte[] _sendBuffer;

        public DccTransfer(string hostname, int port)
        {
            _client = new TcpClient(hostname, port);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task AcceptAsync(Stream stream, long offset, long size)
        {
            await Task.Run(() => Accept(stream, offset, size));
        }

        public void Accept(Stream stream, long offset, long size)
        {
            Seek(stream, offset);

            var networkStream = _client.GetStream();

            OnTransferBegun();

            long bytesTransferred = 0;
            int bytesRead;

            do
            {
                bytesTransferred += bytesRead = ProcessPacket(networkStream, stream);
                SendAcknowledgement(bytesTransferred, networkStream);
                OnTransferProgress(new DccTransferProgressEventArgs(bytesTransferred, size));
            } while (bytesRead > 0 && bytesTransferred < size);

            if (bytesTransferred == size)
            {
                OnTransferComplete();
            }
            else
            {
                OnTransferFailed();
            }
        }

        private static void Seek(Stream stream, long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        private  int ProcessPacket(NetworkStream networkStream, Stream stream)
        {
            var bytesRead = networkStream.Read(_recvBuffer, 0, _recvBuffer.Length);
            stream.Write(_recvBuffer, 0, bytesRead);
            return bytesRead;
        }

        private void SendAcknowledgement(long bytesTransferred, NetworkStream networkStream)
        {
            _sendBuffer = StandardiseEndian(BitConverter.GetBytes(bytesTransferred));
            networkStream.WriteAsync(_sendBuffer, 0, _sendBuffer.Length);
        }

        private static byte[] StandardiseEndian(byte[] bytes)
        {
            return BitConverter.IsLittleEndian ? bytes.Reverse().ToArray() : bytes;
        }

        public event DccTransferEventHandler TransferBegun;
        public event DccTransferEventHandler TransferFailed;
        public event DccTransferEventHandler TransferComplete;
        public event DccTransferProgressEventHandler TransferProgress;

        protected virtual void OnTransferBegun()
        {
            var handler = TransferBegun;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnTransferFailed()
        {
            var handler = TransferFailed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnTransferComplete()
        {
            var handler = TransferComplete;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnTransferProgress(DccTransferProgressEventArgs args)
        {
            var handler = TransferProgress;
            if (handler != null) handler(this, args);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Close();
                    _client = null;
                }
            }
        }

        ~DccTransfer()
        {
            Dispose(false);
        }
    }
}