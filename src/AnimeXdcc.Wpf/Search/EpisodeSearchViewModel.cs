using AnimeXdcc.Wpf.Infrastructure.Bindable;
using Intel.Haruhichan.ApiClient.Clients;

namespace AnimeXdcc.Wpf.Search
{
    /// <summary>
    ///     Search engine view for single episode
    /// </summary>
    internal class EpisodeSearchViewModel : BindableBase
    {
        private IIntelHttpClient _intelHttpClient;

        public EpisodeSearchViewModel(IIntelHttpClient intelHttpClient)
        {
            _intelHttpClient = intelHttpClient;
        }
    }
}