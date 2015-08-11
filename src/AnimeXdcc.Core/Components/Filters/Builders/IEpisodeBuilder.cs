namespace AnimeXdcc.Core.Components.Filters.Builders
{
    public interface IEpisodeBuilder<out T>
    {
        T WithStatic(string term);
        T CaptureEpisode();
        T CaptureVersion();
        T CaptureCrc();
        string Build();
    }
}