using AnimeXdcc.Client.Console.Configuration;
using AnimeXdcc.Common.Logging;

namespace AnimeXdcc.Client.Console
{
    public class Program
    {
        static void Main(string[] args)
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
                new ConsoleLogger(ConsoleLogger.Level.Debug)
                );

            animeXdccClient.Run().Wait();
        }
    }
}
