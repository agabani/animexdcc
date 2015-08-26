using AnimeXdcc.Wpf.Download;
using AnimeXdcc.Wpf.General;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.DependencyInjection;
using AnimeXdcc.Wpf.Infrastructure.DependencyInjection.Unity;
using AnimeXdcc.Wpf.Infrastructure.Relay;
using AnimeXdcc.Wpf.Search;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly IDependencyResolver _dependencyResolver = new UnityFactory().Create();
        private readonly DownloadEpisodeViewModel _downloadEpisodeViewModel;
        private readonly EpisodeSearchResultsViewModel _episodeSearchResultsViewModel;
        private readonly EpisodeSearchViewModel _episodeSearchViewModel;
        private readonly HomeViewModel _homeViewModel;
        private BindableBase _currentViewModel;

        public MainWindowViewModel()
        {
            NavigationCommand = new RelayCommand<string>(OnNavigation);

            _homeViewModel = _dependencyResolver.GetSerivce<HomeViewModel>();
            _episodeSearchViewModel = _dependencyResolver.GetSerivce<EpisodeSearchViewModel>();
            _episodeSearchResultsViewModel = _dependencyResolver.GetSerivce<EpisodeSearchResultsViewModel>();
            _downloadEpisodeViewModel = _dependencyResolver.GetSerivce<DownloadEpisodeViewModel>();

            _homeViewModel.EpisodeNavigationRequested += () => OnNavigation("SearchEpisode");
            _episodeSearchViewModel.SearchRequested += OnSearchRequested;
            _episodeSearchResultsViewModel.DownloadRequested += OnDownloadRequested;

            CurrentViewModel = _homeViewModel;
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
                case "Home":
                    CurrentViewModel = _homeViewModel;
                    break;
                case "SearchEpisode":
                    CurrentViewModel = _episodeSearchViewModel;
                    break;
                default:
                    CurrentViewModel = _homeViewModel;
                    break;
            }
        }

        private void OnSearchRequested(string searchTerm)
        {
            _episodeSearchResultsViewModel.SearchTerm = searchTerm;
            CurrentViewModel = _episodeSearchResultsViewModel;
        }

        private void OnDownloadRequested(Package package)
        {
            _downloadEpisodeViewModel.Package = package;
            CurrentViewModel = _downloadEpisodeViewModel;
        }
    }
}