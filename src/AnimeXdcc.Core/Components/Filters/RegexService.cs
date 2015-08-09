using System.Text.RegularExpressions;

namespace AnimeXdcc.Core.Components.Filters
{
    public class RegexService
    {
        private readonly string _pattern;

        public RegexService(string pattern)
        {
            _pattern = pattern;
        }

        public EpisodeDetail ParseFileName(string fileName)
        {
            var match = Regex.Match(fileName, _pattern, RegexOptions.IgnoreCase);

            if (!match.Success || MatchesOutOfRange(match.Groups.Count))
            {
                return null;
            }

            var episode = long.Parse(match.Groups[1].Value);
            var version = match.Groups.Count == 3 ? long.Parse(match.Groups[2].Value) : 1;

            return new EpisodeDetail(fileName, episode, version);
        }

        private static bool MatchesOutOfRange(int count)
        {
            return count > 3 || count < 2;
        }
    }

    public class EpisodeDetail
    {
        public EpisodeDetail(string fullName, long episode, long version)
        {
            FullName = fullName;
            Episode = episode;
            Version = version;
        }

        public string FullName { get; private set; }
        public long Episode { get; private set; }
        public long Version { get; private set; }
    }
}
