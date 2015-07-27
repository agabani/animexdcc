using AnimeXdcc.Core.Irc;
using AnimeXdcc.Core.Logging;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Irc
{
    [TestFixture]
    public class ProgramLifeTest
    {
        [Test]
        public void Should_be_able_to_stay_alive_indefinately()
        {
            using (
                var xdccIrcClient = new XdccIrcClient("speech", "speech", "speech", "irc.rizon.net", 6667, "#intel",
                    new TraceLogger(TraceLogger.Level.Debug)))
            {
                xdccIrcClient.Run();
            }
        }
    }
}