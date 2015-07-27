using Generic.IrcClient;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Irc
{
    [TestFixture]
    public class IpConverterTest
    {
        [Test]
        public void Should_be_able_to_convert_ip_address_to_int_address()
        {
            const string ipAddress = "202.186.13.4";

            var intAddress = new IpConverter().IpAddressToIntAddress(ipAddress);

            Assert.That(intAddress, Is.EqualTo(3401190660));
        }

        [Test]
        public void Should_be_able_to_convert_int_address_to_ip_address()
        {
            const long intAddress = 3401190660;

            var ipAddress = new IpConverter().IntAddressToIpAddress(intAddress);

            Assert.That(ipAddress, Is.EqualTo("202.186.13.4"));
        }
    }
}