namespace AnimeXdcc.Core.Dcc.Components
{
    public interface IDccClientFactory
    {
        IDccClient Create(long fileSize);
    }
}