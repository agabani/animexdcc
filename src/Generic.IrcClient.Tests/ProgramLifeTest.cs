using AnimeXdcc.Common.Logging;
using NUnit.Framework;

namespace Generic.IrcClient.Tests
{
    [TestFixture]
    public class ProgramLifeTest
    {
        [Test]
        public void Should_be_able_to_stay_alive_indefinately()
        {
            var xdccIrcClient = new XdccIrcClient("speech", "speech", "speech", "irc.rizon.net", 6667, "#intel",
                new TraceLogger(TraceLogger.Level.Debug));

            xdccIrcClient.Run();
        }
    }
}