using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IrcDotNet;

namespace AnimeXdcc.Core.Clients.Irc.Components
{
    public class IrcClient : IIrcClient
    {
        public enum IrcFailureKind
        {
            None,
            TaskCancelled,
            ServerNotFound,
            SourceNotFound
        }

        private readonly string _hostname;
        private readonly string _nickname;
        private readonly int _port;
        private StandardIrcClient _standardIrcClient = new StandardIrcClient();

        public IrcClient(string hostname, int port, string nickname)
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

        public async Task<IrcResult> RequestPackageAsync(string botName, int packageId,
            CancellationToken token = default(CancellationToken))
        {
            var connectResult = await ConnectAsync(token);

            if (!connectResult.Successful)
            {
                return connectResult;
            }

            var channelResult = await FindTargetChannel(botName, token);

            if (!channelResult.Successful)
            {
                return channelResult;
            }

            var joinResult = await JoinChannel(channelResult.Result, token);

            if (!joinResult.Successful)
            {
                return joinResult;
            }

            var requestResultTask = RequestPackageTransfer(botName, packageId, token);
            var recieveResult = await RecievePackageTransfer(botName, token);
            var requestResult = await requestResultTask;

            if (!requestResult.Successful)
            {
                return requestResult;
            }

            if (!recieveResult.Successful)
            {
                return recieveResult;
            }

            return recieveResult;
        }

        ~IrcClient()
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

        private async Task<IrcResult> ConnectAsync(CancellationToken token)
        {
            if (_standardIrcClient.IsRegistered)
            {
                return new IrcResult(true, string.Format("{0}:{1}", _hostname, _port));
            }

            _standardIrcClient.Connect(_hostname, _port, false, new IrcUserRegistrationInfo
            {
                NickName = _nickname,
                Password = null,
                RealName = _nickname,
                UserName = _nickname
            });

            while (!_standardIrcClient.IsRegistered && !token.IsCancellationRequested)
            {
                await Delay(token);
            }

            if (_standardIrcClient.IsRegistered == false)
            {
                return new IrcResult(false, null, IrcFailureKind.ServerNotFound);
            }

            if (token.IsCancellationRequested)
            {
                return new IrcResult(false, null, IrcFailureKind.TaskCancelled);
            }

            return new IrcResult(true, string.Format("{0}:{1}", _hostname, _port));
        }

        private async Task<IrcResult> FindTargetChannel(string target, CancellationToken token)
        {
            IrcResult result = null;

            EventHandler<IrcUserEventArgs> standardIrcClientOnWhoIsReplyReceived = (sender, args) =>
            {
                var ircChannel = args.User.Client.Channels.FirstOrDefault();

                result = ircChannel == null
                    ? new IrcResult(false, target, IrcFailureKind.SourceNotFound)
                    : new IrcResult(true, ircChannel.Name);
            };

            _standardIrcClient.WhoIsReplyReceived += standardIrcClientOnWhoIsReplyReceived;

            _standardIrcClient.QueryWhoIs(target);

            while (result == null && !token.IsCancellationRequested)
            {
                await Delay(token);
            }

            _standardIrcClient.WhoIsReplyReceived -= standardIrcClientOnWhoIsReplyReceived;

            if (token.IsCancellationRequested)
            {
                return new IrcResult(false, target, IrcFailureKind.TaskCancelled);
            }

            return result;
        }

        private async Task<IrcResult> JoinChannel(string channel, CancellationToken token)
        {
            IrcResult result = null;

            if (_standardIrcClient.Channels.Any(c => c.Name == channel) &&
                _standardIrcClient.Channels.First(c => c.Name == channel)
                    .Users.Any(u => u.User.NickName == _standardIrcClient.LocalUser.NickName))
            {
                return new IrcResult(true, channel);
            }

            EventHandler<IrcChannelEventArgs> localUserOnJoinedChannel = (sender, args) =>
            {
                if (channel.Equals(args.Channel.Name, StringComparison.OrdinalIgnoreCase))
                {
                    result = new IrcResult(true, channel);
                }
            };

            _standardIrcClient.LocalUser.JoinedChannel += localUserOnJoinedChannel;

            _standardIrcClient.Channels.Join(channel);

            while (result == null && !token.IsCancellationRequested)
            {
                await Delay(token);
            }

            _standardIrcClient.LocalUser.JoinedChannel -= localUserOnJoinedChannel;

            if (token.IsCancellationRequested)
            {
                return new IrcResult(false, channel, IrcFailureKind.TaskCancelled);
            }

            return new IrcResult(true, channel);
        }

        private async Task<IrcResult> RequestPackageTransfer(string target, int packageId, CancellationToken token)
        {
            IrcResult result = null;

            EventHandler<IrcMessageEventArgs> localUserOnMessageSent =
                (sender, args) => { result = new IrcResult(true, string.Format("{0} {1}", target, packageId)); };

            _standardIrcClient.LocalUser.MessageSent += localUserOnMessageSent;

            _standardIrcClient.LocalUser.SendMessage(target, string.Format("XDCC SEND #{0}", packageId));

            while (result == null && !token.IsCancellationRequested)
            {
                await Delay(token);
            }

            _standardIrcClient.LocalUser.MessageSent -= localUserOnMessageSent;

            if (token.IsCancellationRequested)
            {
                return new IrcResult(false, string.Format("{0} {1}", target, packageId), IrcFailureKind.TaskCancelled);
            }

            return result;
        }

        private async Task<IrcResult> RecievePackageTransfer(string target, CancellationToken token)
        {
            IrcResult result = null;

            EventHandler<IrcMessageEventArgs> localUserOnMessageReceived = (sender, args) =>
            {
                if (args.Source.Name.Equals(target, StringComparison.OrdinalIgnoreCase))
                {
                    result = new IrcResult(true, args.Text);
                }
            };

            _standardIrcClient.LocalUser.MessageReceived += localUserOnMessageReceived;

            while (result == null && !token.IsCancellationRequested)
            {
                await Delay(token);
            }

            _standardIrcClient.LocalUser.MessageReceived -= localUserOnMessageReceived;

            if (token.IsCancellationRequested)
            {
                return new IrcResult(false, target, IrcFailureKind.TaskCancelled);
            }

            return result;
        }

        private static Task Delay(CancellationToken cancellationToken)
        {
            return Task.Delay(100, cancellationToken);
        }

        public class IrcResult
        {
            public IrcResult(bool successful, string result, IrcFailureKind failureKind = IrcFailureKind.None)
            {
                Successful = successful;
                Result = result;
                FailureKind = failureKind;
            }

            public bool Successful { get; private set; }
            public string Result { get; private set; }
            public IrcFailureKind FailureKind { get; private set; }
        }
    }
}