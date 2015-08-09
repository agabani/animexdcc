namespace AnimeXdcc.Core.Components.Filters.Builders
{
    public class RegexPatternBuilder
    {
        private string _pattern;

        public RegexPatternBuilder()
        {
            _pattern = string.Empty;
        }

        public RegexPatternBuilder WithStatic(string term)
        {
            _pattern += term
                .Replace(@"[", @"\[").Replace(@"]", @"\]");

            return this;
        }

        public RegexPatternBuilder CaptureEpisode()
        {
            _pattern += @"(\d+)";

            return this;
        }

        public RegexPatternBuilder CaptureVersion()
        {
            _pattern += @"v?(\d+)?";

            return this;
        }

        public RegexPatternBuilder CaptureCrc()
        {
            _pattern += @"([A-F0-9]{8})";
            return this;
        }

        public string Build()
        {
            return _pattern;
        }
    }
}