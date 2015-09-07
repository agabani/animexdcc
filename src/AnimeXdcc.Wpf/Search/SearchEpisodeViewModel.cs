using System;
using System.Collections.Generic;
using System.Threading;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Relay;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services.Search;

namespace AnimeXdcc.Wpf.Search
{
    internal class SearchEpisodeViewModel : BindableBase
    {
        private readonly ISearchService _search;
        private List<DccSearchResults> _searchResults;
        private string _searchTerm;
        private CancellationTokenSource _source;

        public SearchEpisodeViewModel(ISearchService search)
        {
            if (search == null)
            {
                throw new ArgumentNullException("search");
            }

            _search = search;

            DownloadCommand = new RelayCommand<DccSearchResults>(DownloadAsync);
            SearchCommand = new RelayCommand(SearchAsync);
        }

        public List<DccSearchResults> SearchResults
        {
            get { return _searchResults; }
            set { SetProperty(ref _searchResults, value); }
        }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        public RelayCommand<DccSearchResults> DownloadCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }

        private void DownloadAsync(DccSearchResults searchResults)
        {
            DownloadRequested(searchResults);
        }

        private async void SearchAsync()
        {
            if (_source != null)
            {
                _source.Cancel();
                _source.Dispose();
                _source = null;
            }

            _source = new CancellationTokenSource();

            SearchResults = await _search.SearchAsync(SearchTerm, _source.Token);

            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
        }

        public event Action<DccSearchResults> DownloadRequested = delegate { };
    }
}