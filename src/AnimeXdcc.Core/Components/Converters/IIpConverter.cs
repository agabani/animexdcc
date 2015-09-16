namespace AnimeXdcc.Core.Components.Converters
{
    public interface IIpConverter
    {
        long IpAddressToIntAddress(string ipAddress);
        string IntAddressToIpAddress(long intAddress);
    }
}