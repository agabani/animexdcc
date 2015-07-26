using NUnit.Framework;

namespace Integration.IrcClient
{
    [TestFixture]
    public class Sample
    {
        [Test]
        public void Join_channel()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.Join(new[] {"#speech", "#speechless"});
        }

        [Test]
        public void Leave_channel()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.Join(new[] {"#speech", "#speechless"});
            ircClient.Leave(new[] {"#speech", "#speechless"});
        }

        [Test]
        public void Watch_join_channel()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.Join(new[] {"#speech", "#speechless"});
            ircClient.WatchJoin(new[] {"#speech", "#speechless"}, "speechless");
        }

        [Test]
        public void Watch_leave_channel()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.Join(new[] {"#speech", "#speechless"});
            ircClient.WatchLeave(new[] {"#speech", "#speechless"}, "speechless");
        }

        [Test]
        public void Send_channel_message()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.Join(new[] {"#speech", "#speechless"});
            ircClient.SendChannelMessage("#speech", "channel message");
            ircClient.SendChannelMessage("#speechless", "channel message");
        }

        [Test]
        public void Recieve_channel_message()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.Join(new[] { "#speech", "#speechless" });
            ircClient.ReceiveChannelMessage("speechless", "#speech", "channel message");
            ircClient.ReceiveChannelMessage("speechless", "#speechless", "channel message");
        }

        [Test]
        public void Send_privage_message()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.SendPrivateMessage("speechless", "private message");
        }

        [Test]
        public void Recieve_privage_message()
        {
            var ircClient = new IrcClient();
            ircClient.Connect("irc.rizon.net", 6667, false, "IntegrationIrcClient", null);

            ircClient.RecievePrivateMessage("speechless", "private message");
        }
    }
}