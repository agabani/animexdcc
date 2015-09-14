namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public interface IDccTransferFactory
    {
        IDccTransfer Create(string hostname, int port);
    }
}