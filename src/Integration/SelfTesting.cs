using System;
using System.Linq;
using System.Threading.Tasks;
using Integration.Clients;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    public class SelfTesting
    {
        private const string ObserverName = "ObserverIrcClient";
        private const string PractitionerName = "PractitionerIrcClient";
        private readonly string[] _channels = {"#speech", "#speechless"};
        private IntegrationIrcClient _observer;
        private IntegrationIrcClient _practitioner;

        [Test]
        public async Task Should_join_channel()
        {
            await Init();
            var watchJoin = _observer.WatchJoin(_channels, PractitionerName);
            await _practitioner.Join(_channels);
            await watchJoin;
        }

        [Test]
        public async Task Should_leave_channel()
        {
            await Init();
            var watchLeave = _observer.WatchLeave(_channels, PractitionerName);
            await _practitioner.Join(_channels);
            await _practitioner.Leave(_channels);
            await watchLeave;
        }

        [Test]
        public async Task Should_send_channel_message()
        {
            await Init();
            var message = RandomMessage();
            var target = _channels.First();
            var receive = _observer.ReceiveChannelMessage(PractitionerName, target, message);
            await _practitioner.Join(_channels);
            await _practitioner.SendChannelMessage(target, message);
            await receive;
        }

        [Test]
        public async Task Should_send_private_message()
        {
            await Init();
            var message = RandomMessage();
            var recieve = _observer.RecievePrivateMessage(PractitionerName, message);
            await _practitioner.SendPrivateMessage(ObserverName, message);
            await recieve;
        }

        /// <summary>
        ///     NUnit 2.x does not support async test fixtures, this functionality is expected to be added in 3.0
        ///     This function is a substitute until NUnit 3.0 is released.
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            const string hostname = "irc.synirc.net";
            const int port = 6667;
            const bool useSsl = false;

            if (_observer == null)
            {
                _observer = new IntegrationIrcClient();

                await _observer.Connect(hostname, port, useSsl, ObserverName, null);
                await _observer.Join(_channels);
            }

            if (_practitioner == null)
            {
                _practitioner = new IntegrationIrcClient();
                await _practitioner.Connect(hostname, port, useSsl, PractitionerName, null);
            }
        }

        private static string RandomMessage()
        {
            return Guid.NewGuid().ToString();
        }
    }
}