using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Generic.DccClient
{
    public class XdccDccClient
    {
        public void Download(string ipAddress, int port, uint filesize, string path)
        {
            Console.WriteLine("Starting download");

            BinaryWriter file;
            using (var tcpClient = new TcpClient(ipAddress, port))
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    int bytes;

                    var totalBytes = 0;

                    file = new BinaryWriter(File.OpenWrite(path));

                    var buffer = new byte[8192];

                    Console.WriteLine("Connection opened");

                    do
                    {
                        bytes = networkStream.Read(buffer, 0, buffer.Length);

                        if (bytes > 0)
                        {
                            file.Write(buffer, 0, bytes);
                            totalBytes += bytes;
                        }

                        if (totalBytes == filesize)
                        {
                            break;
                        }
                    } while (bytes > 0);

                    Console.WriteLine("Connection closed");
                }
            }

            Console.WriteLine("Finished download");

            file.Close();
        }
    }
}