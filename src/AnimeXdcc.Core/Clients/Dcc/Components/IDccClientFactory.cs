namespace AnimeXdcc.Core.Clients.Dcc.Components
{
    public interface IDccClientFactory
    {
        IDccClient Create(long fileSize);
    }
}