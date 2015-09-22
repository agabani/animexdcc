using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AnimeXdcc.Core.Clients.Models;
using AnimeXdcc.Core.Services;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Relay;

namespace AnimeXdcc.Wpf.Search
{
    internal class SearchEpisodeViewModel : BindableBase
    {
        private readonly ISearchService _search;
        private string _displayNoResultsFoundMessage = "Hidden";
        private bool _enableProgressRing;
        private bool _enableResults;
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

        public bool EnableResults
        {
            get { return _enableResults; }
            set { SetProperty(ref _enableResults, value); }
        }

        public bool EnableProgressRing
        {
            get { return _enableProgressRing; }
            set { SetProperty(ref _enableProgressRing, value); }
        }

        public string DisplayNoResultsFoundMessage
        {
            get { return _displayNoResultsFoundMessage; }
            set { SetProperty(ref _displayNoResultsFoundMessage, value); }
        }

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

            DisableUi();

            SearchResults = await _search.SearchAsync(SearchTerm, _source.Token);

            EnableUi();

            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }
        }

        private void DisableUi()
        {
            DisplayNoResultsFoundMessage = "Hidden";
            EnableResults = false;
            EnableProgressRing = true;
        }

        private void EnableUi()
        {
            DisplayNoResultsFoundMessage = SearchResults == null || SearchResults.Any() ? "Hidden" : "Visible";
            EnableProgressRing = false;
            EnableResults = true;
        }

        public event Action<DccSearchResults> DownloadRequested = delegate { };
    }
}