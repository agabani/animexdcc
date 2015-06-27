using System.Linq;
using NUnit.Framework;

namespace Generic.IrcClient.Tests
{
    [TestFixture]
    public class IpConverterTest
    {
        [Test]
        public void Should_be_able_to_convert_ip_address_to_uint_address()
        {
            const string ipAddress = "202.186.13.4";

            var intAddress = new IpConverter().IpAddressToUintAddress(ipAddress);

            Assert.That(intAddress, Is.EqualTo(3401190660));
        }

        [Test]
        public void Should_be_able_to_convert_uint_address_to_ip_address()
        {
            const uint uintAddress = 3401190660;

            var ipAddress = new IpConverter().UintAddressToIpAddress(uintAddress);

            Assert.That(ipAddress, Is.EqualTo("202.186.13.4"));
        }
    }
}