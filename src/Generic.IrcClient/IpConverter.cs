using System.Linq;

namespace Generic.IrcClient
{
    public class IpConverter
    {
        public uint IpAddressToUintAddress(string ipAddress)
        {
            var subBlocks = ipAddress.Split('.').Select(uint.Parse).ToArray();

            return subBlocks[0]*16777216 + subBlocks[1]*65536 + subBlocks[2]*256 + subBlocks[3];
        }

        public string UintAddressToIpAddress(uint uintAddress)
        {
            return string.Format("{0}.{1}.{2}.{3}",
                (uintAddress/16777216)%256,
                (uintAddress/65536)%256,
                (uintAddress/256)%256,
                (uintAddress)%256
                );
        }
    }
}