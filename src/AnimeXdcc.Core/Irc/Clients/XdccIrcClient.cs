using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IrcDotNet;

namespace AnimeXdcc.Core.Irc.Clients
{
    public class XdccIrcClient : IXdccIrcClient
    {
        private readonly string _hostname;
        private readonly string _nickname;
        private readonly int _port;
        private StandardIrcClient _standardIrcClient = new StandardIrcClient();

        public XdccIrcClient(string hostname, int port, string nickname)
        {
            _port = port;
            _hostname = hostname;
            _nickname = nickname;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<string> RequestPackageAsync(string target, int packageId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await ConnectAsync(cancellationToken);

            var channel = await FindTargetChannel(target, cancellationToken);
            await JoinChannel(channel, cancellationToken);

            await RequestPackageTransfer(target, packageId, cancellationToken);
            return await RecievePackageTransfer(target, cancellationToken);
        }

        ~XdccIrcClient()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_standardIrcClient != null)
                {
                    _standardIrcClient.Dispose();
                    _standardIrcClient = null;
                }
            }
        }

        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (_standardIrcClient.IsRegistered)
            {
                return;
            }

            _standardIrcClient.Connect(_hostname, _port, false, new IrcUserRegistrationInfo
            {
                NickName = _nickname,
                Password = null,
                RealName = _nickname,
                UserName = _nickname
            });

            while (!_standardIrcClient.IsRegistered && !cancellationToken.IsCancellationRequested)
            {
                await Delay(cancellationToken);
            }
        }

        private async Task<string> FindTargetChannel(string target, CancellationToken cancellationToken)
        {
            string channel = null;

            _standardIrcClient.WhoIsReplyReceived +=
                (sender, args) => { channel = args.User.Client.Channels.First().Name; };

            _standardIrcClient.QueryWhoIs(target);

            while (channel == null && !cancellationToken.IsCancellationRequested)
            {
                await Delay(cancellationToken);
            }

            return channel;
        }

        private async Task JoinChannel(string channel, CancellationToken cancellationToken)
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

            while (!joinedChannel && !cancellationToken.IsCancellationRequested)
            {
                await Delay(cancellationToken);
            }
        }

        private async Task RequestPackageTransfer(string target, int packageId, CancellationToken cancellationToken)
        {
            var privateMessageSent = false;

            _standardIrcClient.LocalUser.MessageSent += (sender, args) => { privateMessageSent = true; };

            _standardIrcClient.LocalUser.SendMessage(target, string.Format("XDCC SEND #{0}", packageId));

            while (!privateMessageSent && !cancellationToken.IsCancellationRequested)
            {
                await Delay(cancellationToken);
            }
        }

        private async Task<string> RecievePackageTransfer(string target, CancellationToken cancellationToken)
        {
            string messageReceived = null;

            _standardIrcClient.LocalUser.MessageReceived += (sender, args) =>
            {
                if (args.Source.Name.Equals(target, StringComparison.OrdinalIgnoreCase))
                {
                    messageReceived = args.Text;
                }
            };

            while (messageReceived == null && !cancellationToken.IsCancellationRequested)
            {
                await Delay(cancellationToken);
            }

            return messageReceived;
        }

        private static Task Delay(CancellationToken cancellationToken)
        {
            return Task.Delay(100, cancellationToken);
        }
    }
}