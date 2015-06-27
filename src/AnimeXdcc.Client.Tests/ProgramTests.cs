using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Client.Tests.Configuration;
using Generic.DccClient;
using Generic.IrcClient;
using Intel.Haruhichan.ApiClient.Client;
using NUnit.Framework;

namespace AnimeXdcc.Client.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public async Task Program()
        {
            var configuration = new ConfigurationManager();

            var searchString = configuration.Xdcc.SearchTerm;

            Console.WriteLine("[SEARCH] {0}", searchString);

            var intelHttpClient = new IntelHttpClient(new Uri(configuration.Xdcc.BaseUrl));
            var search = await intelHttpClient.Search(searchString);

            Console.WriteLine("[RESULT] {0} matches found", search.Files.Count());

            Assert.That(search.Error, Is.False);

            var package = search.Files.Aggregate((p1, p2) => p1.Requested > p2.Requested ? p1 : p2);

            Console.WriteLine("[SELECTED] \"{0}\" from {1}. Package Id: {2}. Size: {3}. Requested: {4}.",
                package.FileName, package.BotName, package.PackageNumber, package.Size, package.Requested);

            var xdccIrcClient = new XdccIrcClient(
                configuration.Irc.User.NickName,
                configuration.Irc.User.RealName,
                configuration.Irc.User.UserName,
                configuration.Irc.Server.HostName,
                configuration.Irc.Server.Port,
                configuration.Irc.Channel);

            xdccIrcClient.DccSendReceived += (sender, message) =>
            {
                var xdccDccClient = new XdccDccClient();
                xdccDccClient.Download(message.IpAddress, message.Port, message.FileSize, message.FileName);
                Environment.Exit(0);
            };

            xdccIrcClient.Run();

            Thread.Sleep(3000);

            xdccIrcClient.RequestPackage(package.BotName, package.PackageNumber);

            while (true)
            {
                
            }
        }
    }
}