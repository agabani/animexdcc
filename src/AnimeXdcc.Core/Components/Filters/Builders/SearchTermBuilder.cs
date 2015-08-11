namespace AnimeXdcc.Core.Components.Filters.Builders
{
    public class SearchTermBuilder : IEpisodeBuilder<SearchTermBuilder>
    {
        private int _counter;
        private string _pattern;

        public SearchTermBuilder()
        {
            _pattern = string.Empty;
        }

        public SearchTermBuilder WithStatic(string term)
        {
            _pattern += term;

            return this;
        }

        public SearchTermBuilder CaptureEpisode()
        {
            _pattern += string.Format("{0}{1}{2}", "{", _counter++, "}");

            return this;
        }

        public SearchTermBuilder CaptureVersion()
        {
            _pattern += string.Format("v{0}{1}{2}", "{", _counter++, "}");

            return this;
        }

        public SearchTermBuilder CaptureCrc()
        {
            _pattern += " ";
            _counter++;

            return this;
        }

        public string Build()
        {
            return _pattern;
        }
    }
}