namespace AnimeXdcc.Core.Dcc.Components
{
    public interface IDccTransferFactory
    {
        IDccTransfer Create(string hostname, int port);
    }
}