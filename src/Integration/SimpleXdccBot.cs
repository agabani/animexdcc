using System.IO;
using System.Threading.Tasks;
using AnimeXdcc.Core.Irc;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class SimpleXdccBot
    {
        [Test]
        public async Task HostFile()
        {
            var ircClient = new IrcClient();

            await ircClient.Connect("irc.rizon.net", 6667, false, "ObserverXdccServer", null);

            await ircClient.Join(new[] {"#speechless"});

            await ircClient.RecievePrivateMessage("speechless", "xdcc send anything");

            var dccClient = new DccClient();

            using (var file = File.OpenRead(@"sample.txt"))
            {
                var dccSendTask = dccClient.Send(12345, file, file.Length);

                var ipConverter = new IpConverter();

                await ircClient.SendPrivateMessage("speechless", string.Format("\x01" + "DCC SEND samplefile.txt {0} {1} {2}" + "\x01", ipConverter.IpAddressToIntAddress("127.0.0.1"), 12345, file.Length));

                await dccSendTask;

                file.Dispose();
            }
        }
    }
}