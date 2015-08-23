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
        private BindableBase _currentViewModel;

        public MainWindowViewModel()
        {
            NavigationCommand = new RelayCommand<string>(OnNavigation);

            _episodeSearchViewModel = _dependencyResolver.GetSerivce<EpisodeSearchViewModel>();

            _currentViewModel = _episodeSearchViewModel;
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