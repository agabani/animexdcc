using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IrcDotNet;

namespace Integration.Clients
{
    public class IntegrationIrcClient : IDisposable
    {
        private readonly StandardIrcClient _ircClient;

        public IntegrationIrcClient()
        {
            _ircClient = new StandardIrcClient();
        }

        public void Dispose()
        {
            _ircClient.Dispose();
        }

        public async Task Connect(string hostname, int port, bool useSsl, string name, string password)
        {
            _ircClient.Connect(hostname, port, useSsl,
                new IrcUserRegistrationInfo
                {
                    NickName = name,
                    RealName = name,
                    UserName = name,
                    Password = password
                });

            var registered = false;

            _ircClient.Registered += (sender, args) => { registered = true; };

            while (!registered)
            {
                await Sleep();
            }
        }

        public async Task Disconnect()
        {
            var disconnected = false;

            _ircClient.Disconnected += (sender, args) => { disconnected = true; };

            _ircClient.Disconnect();

            while (!disconnected)
            {
                await Sleep();
            }
        }

        public async Task Join(string[] channels)
        {
            _ircClient.Channels.Join(channels);

            var joinedChannels = new HashSet<string>();

            foreach (var channel in _ircClient.Channels.Where(channel => channels.Contains(channel.Name)))
            {
                joinedChannels.Add(channel.Name);
            }

            _ircClient.LocalUser.JoinedChannel += (sender, args) => { joinedChannels.Add(args.Channel.Name); };

            while (!joinedChannels.SetEquals(channels))
            {
                await Sleep();
            }
        }

        public async Task WatchJoin(string[] channels, string nickname)
        {
            var joinedChannels = new HashSet<string>();

            foreach (var channel in channels.Select(channel => _ircClient.Channels.First(c => c.Name == channel)))
            {
                channel.UserJoined += (sender, args) =>
                {
                    if (args.ChannelUser.User.NickName == nickname)
                    {
                        joinedChannels.Add(args.ChannelUser.Channel.Name);
                    }
                };
            }

            while (!joinedChannels.SetEquals(channels))
            {
                await Sleep();
            }
        }

        public async Task Leave(string[] channels)
        {
            _ircClient.Channels.Leave(channels);

            var leftChannels = new HashSet<string>();

            _ircClient.LocalUser.LeftChannel += (sender, args) => { leftChannels.Add(args.Channel.Name); };

            while (!leftChannels.SetEquals(channels))
            {
                await Sleep();
            }
        }

        public async Task WatchLeave(string[] channels, string nickname)
        {
            var leftChannels = new HashSet<string>();

            foreach (var channel in channels.Select(channel => _ircClient.Channels.First(c => c.Name == channel)))
            {
                channel.UserLeft += (sender, args) =>
                {
                    if (args.ChannelUser.User.NickName == nickname)
                    {
                        leftChannels.Add(args.ChannelUser.Channel.Name);
                    }
                };
            }

            while (!leftChannels.SetEquals(channels))
            {
                await Sleep();
            }
        }

        public async Task SendChannelMessage(string target, string message)
        {
            var sent = false;

            _ircClient.LocalUser.MessageSent += (sender, args) => { sent = true; };

            var ircChannel = _ircClient.Channels.First(c => c.Name == target);

            _ircClient.LocalUser.SendMessage(ircChannel, message);

            while (!sent)
            {
                await Sleep();
            }
        }

        public async Task ReceiveChannelMessage(string source, string target, string message)
        {
            var recieved = false;

            _ircClient.Channels.First(n => n.Name == target).MessageReceived += (sender, args) =>
            {
                if (args.Source.Name == source && args.Text == message)
                {
                    recieved = true;
                }
            };

            while (!recieved)
            {
                await Sleep();
            }
        }

        public async Task SendPrivateMessage(string target, string message)
        {
            var sent = false;

            _ircClient.LocalUser.MessageSent += (sender, args) =>
            {
                if (args.Targets.FirstOrDefault(t => t.Name == target) != null && args.Text == message)
                {
                    sent = true;
                }
            };

            _ircClient.LocalUser.SendMessage(target, message);

            while (!sent)
            {
                await Sleep();
            }
        }

        public async Task RecievePrivateMessage(string message)
        {
            var recieved = false;

            _ircClient.LocalUser.MessageReceived += (sender, args) =>
            {
                if (args.Text == message)
                {
                    recieved = true;
                }
            };

            while (!recieved)
            {
                await Sleep();
            }
        }

        public async Task RecievePrivateMessage(string source, string message)
        {
            var recieved = false;

            _ircClient.LocalUser.MessageReceived += (sender, args) =>
            {
                if (args.Source.Name == source && args.Text == message)
                {
                    recieved = true;
                }
            };

            while (!recieved)
            {
                await Sleep();
            }
        }

        private static Task Sleep()
        {
            return Task.Delay(100);
        }
    }
}