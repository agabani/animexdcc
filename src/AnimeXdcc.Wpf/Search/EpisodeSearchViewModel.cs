using System;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Relay;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf.Search
{
    internal class EpisodeSearchViewModel : BindableBase
    {
        private IIntelService _intelService;
        private string _searchTerm = string.Empty;

        public EpisodeSearchViewModel(IIntelService intelService)
        {
            _intelService = intelService;

            SearchCommand = new RelayCommand(OnSearchRequested);
        }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        public RelayCommand SearchCommand { get; private set; }

        private void OnSearchRequested()
        {
            SearchRequested(_searchTerm);
        }

        public event Action<string> SearchRequested = delegate { };
    }
}