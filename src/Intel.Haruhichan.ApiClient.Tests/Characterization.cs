using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Clients;
using NUnit.Framework;

namespace Intel.Haruhichan.ApiClient.Tests
{
    [TestFixture]
    public class Characterization
    {
        private readonly IntelHttpClient _intelHttpClient = new IntelHttpClient(new ConfigurationManager().BaseUri,
            new TraceLogger(TraceLogger.Level.Debug));

        [Test]
        public async Task Should_search()
        {
            const string searchTerms = "Dan Machi 1080p";

            var regex = CreateRegex(searchTerms);
            var result = await _intelHttpClient.SearchAsync(searchTerms);

            Assert.That(result.Error, Is.False);
            Assert.That(result.Files.Any(), Is.True);
            Assert.That(result.Files.Select(f => regex.IsMatch(f.FileName)), Is.All.True);
        }

        [Test]
        public async Task Should_obtain_bot_list()
        {
            var result = await _intelHttpClient.BotAsync(92);

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