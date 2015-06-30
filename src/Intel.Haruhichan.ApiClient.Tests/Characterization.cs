using System.Linq;
using System.Text.RegularExpressions;
using AnimeXdcc.Common.Logging;
using Intel.Haruhichan.ApiClient.Clients;
using NUnit.Framework;

namespace Intel.Haruhichan.ApiClient.Tests
{
    [TestFixture]
    public class Characterization
    {
        private readonly IntelHttpClient _intelHttpClient = new IntelHttpClient(new ConfigurationManager().BaseUri, new TraceLogger(TraceLogger.Level.Debug));

        [Test]
        public void Should_search()
        {
            const string searchTerms = "Dan Machi 1080p";

            var regex = CreateRegex(searchTerms);
            var result = _intelHttpClient.Search(searchTerms).GetAwaiter().GetResult();

            Assert.That(result.Error, Is.False);
            Assert.That(result.Files.Any(), Is.True);
            Assert.That(result.Files.Select(f => regex.IsMatch(f.FileName)), Is.All.True);
        }

        [Test]
        public void Should_obtain_bot_list()
        {
            var result = _intelHttpClient.Bot(92).GetAwaiter().GetResult();

            Assert.That(result.Error, Is.False);
            Assert.That(result.Files.Any(), Is.True);
            Assert.That(result.Files.Select(f => f.BotId), Is.All.EqualTo(92));
        }

        private Regex CreateRegex(string search)
        {
            var terms = search.Split(' ');
            var expression = terms.Aggregate(string.Empty, (current, term) => current + string.Format("(?=.*{0})", term));
            var pattern = string.Format("^{0}.+", expression);
            return new Regex(pattern, RegexOptions.IgnoreCase);
        }
    }
}