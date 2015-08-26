using System;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Relay;

namespace AnimeXdcc.Wpf.General
{
    internal class HomeViewModel : BindableBase
    {
        public HomeViewModel()
        {
            EpisodeCommand = new RelayCommand(OnEpisodeNavigationRequested);
        }

        public RelayCommand EpisodeCommand { get; private set; }
        public event Action EpisodeNavigationRequested = delegate { };

        protected virtual void OnEpisodeNavigationRequested()
        {
            EpisodeNavigationRequested();
        }
    }
}