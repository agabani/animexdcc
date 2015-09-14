using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Wpf.Download;
using AnimeXdcc.Wpf.General;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.DependencyInjection;
using AnimeXdcc.Wpf.Infrastructure.DependencyInjection.Unity;
using AnimeXdcc.Wpf.Infrastructure.Relay;
using AnimeXdcc.Wpf.Search;

namespace AnimeXdcc.Wpf
{
    internal class MainWindowViewModel : BindableBase
    {
        private readonly DownloadQueueViewModel _downloadQueueViewModel;
        private readonly SearchEpisodeViewModel _searchEpisodeViewModel;
        private readonly HomeViewModel _homeViewModel;
        private readonly AboutViewModel _aboutViewModel;
        private BindableBase _currentViewModel;

        public MainWindowViewModel()
        {
            IDependencyResolver dependencyResolver = new UnityFactory().Create();

            NavigationCommand = new RelayCommand<string>(OnNavigation);

            // EXPERIMENT
            _downloadQueueViewModel = dependencyResolver.GetSerivce<DownloadQueueViewModel>();

            _homeViewModel = dependencyResolver.GetSerivce<HomeViewModel>();
            _aboutViewModel = dependencyResolver.GetSerivce<AboutViewModel>();
            _searchEpisodeViewModel = dependencyResolver.GetSerivce<SearchEpisodeViewModel>();

            _homeViewModel.EpisodeNavigationRequested += () => OnNavigation("SearchEpisode");
            _searchEpisodeViewModel.DownloadRequested += OnDownloadRequested;

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
                case "About":
                    CurrentViewModel = _aboutViewModel;
                    break;
                case "Download":
                    CurrentViewModel = _downloadQueueViewModel;
                    break;
                case "SearchEpisode":
                    CurrentViewModel = _searchEpisodeViewModel;
                    break;
                default:
                    CurrentViewModel = _homeViewModel;
                    break;
            }
        }

        private void OnDownloadRequested(DccSearchResults package)
        {
            CurrentViewModel = _downloadQueueViewModel;
            _downloadQueueViewModel.AddToDownloadQueue(package.FileName, package.DccPackages);
        }
    }
}