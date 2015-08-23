using System;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.DependencyInjection;
using AnimeXdcc.Wpf.Infrastructure.DependencyInjection.Unity;
using AnimeXdcc.Wpf.Infrastructure.Relay;
using AnimeXdcc.Wpf.Search;

namespace AnimeXdcc.Wpf
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IDependencyResolver _dependencyResolver = new UnityFactory().Create();
        private readonly EpisodeSearchViewModel _episodeSearchViewModel;
        private readonly EpisodeSearchResultsViewModel _episodeSearchResultsViewModel;
        private BindableBase _currentViewModel;

        public MainWindowViewModel()
        {
            NavigationCommand = new RelayCommand<string>(OnNavigation);

            _episodeSearchViewModel = _dependencyResolver.GetSerivce<EpisodeSearchViewModel>();
            _episodeSearchResultsViewModel = new EpisodeSearchResultsViewModel();

            _episodeSearchViewModel.SearchRequested += OnSearchRequested;

            _currentViewModel = _episodeSearchViewModel;
        }

        private void OnSearchRequested(string searchTerm)
        {
            _episodeSearchResultsViewModel.SearchTerm = searchTerm;
            CurrentViewModel = _episodeSearchResultsViewModel;
        }

        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        public RelayCommand<string> NavigationCommand { get; private set; }

        private void OnNavigation(string destination)
        {
            switch (destination)
            {
                default:
                    CurrentViewModel = _episodeSearchViewModel;
                    break;
            }
        }
    }
}