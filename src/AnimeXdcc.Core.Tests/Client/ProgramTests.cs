using AnimeXdcc.Client;
using AnimeXdcc.Core.Logging;
using AnimeXdcc.Core.Tests.Configuration;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Client
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void Program()
        {
            var configuration = new ConfigurationManager();

            var animeXdccClient = new AnimeXdccClient(
                configuration.Xdcc.SearchTerm,
                configuration.Xdcc.BaseUrl,
                configuration.Irc.Server.HostName,
                configuration.Irc.Server.Port,
                configuration.Irc.User.UserName,
                configuration.Irc.User.RealName,
                configuration.Irc.User.NickName,
                configuration.Irc.Channel,
                new TraceLogger(TraceLogger.Level.Debug)
                );

            animeXdccClient.Run().Wait();
        }
    }
}