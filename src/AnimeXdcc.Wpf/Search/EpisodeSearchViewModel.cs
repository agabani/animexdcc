using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf.Search
{
    /// <summary>
    ///     Search engine view for single episode
    /// </summary>
    internal class EpisodeSearchViewModel : BindableBase
    {
        private IIntelService _intelService;

        public EpisodeSearchViewModel(IIntelService intelService)
        {
            _intelService = intelService;
        }
    }
}