using System;
using System.Linq;
using System.Threading.Tasks;
using IrcDotNet;

namespace AnimeXdcc.Core.Irc.Clients
{
    public class XdccIrcClient : IDisposable
    {
        private readonly string _hostname;
        private readonly string _nickname;
        private readonly int _port;
        private readonly StandardIrcClient _standardIrcClient = new StandardIrcClient();

        public XdccIrcClient(string hostname, int port, string nickname)
        {
            _port = port;
            _hostname = hostname;
            _nickname = nickname;
        }

        public void Dispose()
        {
            if (_standardIrcClient != null)
            {
                _standardIrcClient.Dispose();
            }
        }

        public async Task<string> RequestPackageAsync(string target, int packageId)
        {
            await ConnectAsync();
            var channel = await FindTargetChannel(target);
            await JoinChannel(channel);
            await RequestPackageTransfer(target, packageId);
            return await RecievePackageTransfer(target);
        }

        private async Task ConnectAsync()
        {
            var isRegistered = false;

            _standardIrcClient.Registered += (sender, args) => { isRegistered = true; };

            _standardIrcClient.Connect(_hostname, _port, false, new IrcUserRegistrationInfo
            {
                NickName = _nickname,
                Password = null,
                RealName = _nickname,
                UserName = _nickname
            });

            while (!isRegistered)
            {
                await Delay();
            }
        }

        private async Task<string> FindTargetChannel(string target)
        {
            string channel = null;

            _standardIrcClient.WhoIsReplyReceived +=
                (sender, args) => { channel = args.User.Client.Channels.First().Name; };

            _standardIrcClient.QueryWhoIs(target);

            while (channel == null)
            {
                await Delay();
            }

            return channel;
        }

        private async Task JoinChannel(string channel)
        {
            var joinedChannel = false;

            _standardIrcClient.LocalUser.JoinedChannel += (sender, args) =>
            {
                if (channel.Equals(args.Channel.Name, StringComparison.OrdinalIgnoreCase))
                {
                    joinedChannel = true;
                }
            };

            _standardIrcClient.Channels.Join(channel);

            while (!joinedChannel)
            {
                await Delay();
            }
        }

        private async Task RequestPackageTransfer(string target, int packageId)
        {
            var privateMessageSent = false;

            _standardIrcClient.LocalUser.MessageSent += (sender, args) => { privateMessageSent = true; };

            _standardIrcClient.LocalUser.SendMessage(target, string.Format("XDCC SEND #{0}", packageId));

            while (!privateMessageSent)
            {
                await Delay();
            }
        }

        private async Task<string> RecievePackageTransfer(string target)
        {
            string messageReceived = null;

            _standardIrcClient.LocalUser.MessageReceived += (sender, args) =>
            {
                if (args.Source.Name.Equals(target, StringComparison.OrdinalIgnoreCase))
                {
                    messageReceived = args.Text;
                }
            };

            while (messageReceived == null)
            {
                await Delay();
            }

            return messageReceived;
        }

        private static Task Delay()
        {
            return Task.Delay(100);
        }
    }
}