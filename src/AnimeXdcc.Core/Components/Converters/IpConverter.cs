using System.Linq;

namespace AnimeXdcc.Core.Components.Converters
{
    public class IpConverter
    {
        public long IpAddressToIntAddress(string ipAddress)
        {
            var subBlocks = ipAddress.Split('.').Select(long.Parse).ToArray();

            return subBlocks[0]*16777216 + subBlocks[1]*65536 + subBlocks[2]*256 + subBlocks[3];
        }

        public string IntAddressToIpAddress(long intAddress)
        {
            return string.Format("{0}.{1}.{2}.{3}",
                (intAddress/16777216)%256,
                (intAddress/65536)%256,
                (intAddress/256)%256,
                (intAddress)%256
                );
        }
    }
}