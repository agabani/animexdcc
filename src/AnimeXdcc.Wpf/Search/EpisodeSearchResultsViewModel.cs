using System;
using System.Collections.ObjectModel;
using System.Linq;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Relay;
using AnimeXdcc.Wpf.Services;

namespace AnimeXdcc.Wpf.Search
{
    internal class EpisodeSearchResultsViewModel : BindableBase
    {
        private readonly IIntelService _intelService;
        private ObservableCollection<Package> _searchListing;
        private string _searchTerm;

        public EpisodeSearchResultsViewModel(IIntelService intelService)
        {
            _intelService = intelService;
            _searchListing = new ObservableCollection<Package>();

            SearchCommand = new RelayCommand(SearchAsync);
            DownloadCommand = new RelayCommand<Package>(Download);
        }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                SetProperty(ref _searchTerm, value);
                SearchAsync();
            }
        }

        public ObservableCollection<Package> SearchListing
        {
            get { return _searchListing; }
            set { SetProperty(ref _searchListing, value); }
        }

        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand<Package> DownloadCommand { get; private set; }

        private async void SearchAsync()
        {
            SearchListing =
                new ObservableCollection<Package>((await _intelService.SearchAsync(SearchTerm)).Take(5).ToArray());
        }

        private void Download(Package package)
        {
            DownloadRequested(package);
        }

        public event Action<Package> DownloadRequested = delegate { };
    }
}