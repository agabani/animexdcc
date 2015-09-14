using System;
using System.IO;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Dcc.Components;
using Integration.Clients;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Integration.Components.Dcc
{
    [TestFixture]
    public class DccTransferTests
    {
        [Test]
        public async Task Should_transfer_file()
        {
            var integration = new IntegrationDccClient();

            var inputFileStream = File.OpenRead(@"Data\17 - Nintendo - Mute City Ver. 3.mp3");

            Task.Run(() => integration.Send(12345, inputFileStream, inputFileStream.Length)).GetAwaiter();

            var outputFileStream = File.OpenWrite(@"output.mp3");

            var dccTransfer = new DccTransfer("127.0.0.1", 12345);

            dccTransfer.TransferBegun += (sender, args) => Console.WriteLine("Transfer Begun");
            dccTransfer.TransferFailed += (sender, args) => Console.WriteLine("Transfer Failed");
            dccTransfer.TransferComplete += (sender, args) => Console.WriteLine("Transfer Complete");
            dccTransfer.TransferProgress += (sender, args) => Console.WriteLine("{0}/{1}", args.Transferred, args.Size);

            await dccTransfer.AcceptAsync(outputFileStream, 0, inputFileStream.Length);
        }
    }
}