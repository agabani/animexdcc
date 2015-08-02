using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Clients;
using NUnit.Framework;

namespace Intel.Haruhichan.ApiClient.Tests
{
    [TestFixture]
    public class SeriesTests
    {
        private readonly IntelHttpClient _intelHttpClient = new IntelHttpClient(
            new ConfigurationManager().BaseUri,
            new TraceLogger(TraceLogger.Level.Debug));

        [Test]
        public async Task Should_be_able_to_get_entire_series()
        {
            var series = new Series(_intelHttpClient);
            var files = await series.Obtain("[Doki] Angel Beats Hi10p", new Regex(""));
        }
    }
}