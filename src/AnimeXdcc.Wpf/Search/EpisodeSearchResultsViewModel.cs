using AnimeXdcc.Wpf.Infrastructure.Bindable;

namespace AnimeXdcc.Wpf.Search
{
    internal class EpisodeSearchResultsViewModel : BindableBase
    {
        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }
    }
}