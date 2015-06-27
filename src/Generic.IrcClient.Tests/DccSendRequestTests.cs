using Generic.IrcClient.Dcc;
using NUnit.Framework;

namespace Generic.IrcClient.Tests
{
    [TestFixture]
    public class DccSendRequestTests
    {
        private const string DccMessageString =
            "\x01" + "DCC SEND \"[HorribleSubs] DanMachi - 07 [480p].mkv\" 2731918850 54363 154074956" + "\x01";

        [Test]
        public void Should_be_able_to_identify_dcc_send_message()
        {
            var dccMessageParser = new DccMessageParser(null);

            var isDccMessage = dccMessageParser.IsDccMessage(DccMessageString);

            Assert.That(isDccMessage, Is.True);
        }

        [Test]
        public void Should_be_able_to_parse_dcc_send()
        {
            var dccMessageParser = new DccMessageParser(new IpConverter());

            var dccSendMessage = dccMessageParser.Parse(DccMessageString);

            Assert.That(dccSendMessage.FileSize, Is.EqualTo(154074956));
            Assert.That(dccSendMessage.Port, Is.EqualTo(54363));
            Assert.That(dccSendMessage.IpAddress, Is.EqualTo("162.213.198.2"));
            Assert.That(dccSendMessage.FileName, Is.EqualTo("[HorribleSubs] DanMachi - 07 [480p].mkv"));
        }

        [Test]
        public void Should_be_able_to_parse_dcc_send_without_double_quotes()
        {
            var dccMessageParser = new DccMessageParser(new IpConverter());

            var dccSendMessage = dccMessageParser.Parse(DccMessageString.Replace("\"", string.Empty));

            Assert.That(dccSendMessage.FileSize, Is.EqualTo(154074956));
            Assert.That(dccSendMessage.Port, Is.EqualTo(54363));
            Assert.That(dccSendMessage.IpAddress, Is.EqualTo("162.213.198.2"));
            Assert.That(dccSendMessage.FileName, Is.EqualTo("[HorribleSubs] DanMachi - 07 [480p].mkv"));
        }
    }
}