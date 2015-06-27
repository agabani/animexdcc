using System;
using System.Collections.Generic;
using AnimeXdcc.Common.Logging;
using Generic.IrcClient.Dcc;
using IrcDotNet;

namespace Generic.IrcClient
{
    public class XdccIrcClient
    {
        private const string LogTag = "[XdccIrcClient] ";
        private readonly string _channels;
        private readonly string _hostName;
        private readonly IrcUserRegistrationInfo _ircUserRegistrationInfo;
        private readonly ILogger _logger;
        private readonly int _port;
        private readonly StandardIrcClient _standardIrcClient;

        public XdccIrcClient(string nickName, string realName, string userName, string hostName, int port,
            string channels, ILogger logger)
        {
            _standardIrcClient = new StandardIrcClient();
            _ircUserRegistrationInfo = new IrcUserRegistrationInfo
            {
                NickName = nickName,
                RealName = realName,
                UserName = userName
            };
            _hostName = hostName;
            _port = port;
            _channels = channels;
            _logger = logger;
        }

        public event EventHandler<DccSendMessage> DccSendReceived;

        public void Run()
        {
            _standardIrcClient.Connected += (sender, args) =>
            {
                var client = (StandardIrcClient) sender;
                _logger.Info(LogTag + string.Format("[Connected] {0}", client));
            };

            _standardIrcClient.Registered += (sender, args) =>
            {
                var client = (StandardIrcClient) sender;
                client.Channels.Join(_channels);
                RegisterCallbacks();
            };

            _standardIrcClient.Connect(_hostName, _port, false, _ircUserRegistrationInfo);
        }

        public void RequestPackage(string botName, int packageNumber)
        {
            _standardIrcClient.LocalUser.SendMessage(botName, string.Format("xdcc send #{0}", packageNumber));
        }

        private void RegisterCallbacks()
        {
            _standardIrcClient.LocalUser.JoinedChannel += (sender, args) =>
            {
                var channelEventArgs = args;

                _logger.Info(LogTag + string.Format("[Joined] {0}", channelEventArgs.Channel.Name));

                channelEventArgs.Channel.MessageReceived +=
                    (o, eventArgs) =>
                    {
                        _logger.Info(LogTag +
                                     string.Format("[Channel.MessageReceived] {0} {1}", eventArgs.Source.Name,
                                         eventArgs.GetText()));
                    };

                channelEventArgs.Channel.NoticeReceived +=
                    (o, eventArgs) =>
                    {
                        _logger.Info(LogTag +
                                     string.Format("[Channel.NoticeReceived] {0} {1}", eventArgs.Source.Name,
                                         eventArgs.GetText()));
                    };
            };

            _standardIrcClient.LocalUser.MessageReceived += (sender, args) =>
            {
                var client = (IrcLocalUser) sender;

                var dccMessageParser = new DccMessageParser(new IpConverter());

                if (dccMessageParser.IsDccMessage(args.Text))
                {
                    var result = dccMessageParser.Parse(args.Text);

                    OnDccSendReceived(result);
                }
                else
                {
                    _logger.Info(LogTag +
                                 string.Format("[LocalUser.MessageReceived] {0} {1}", args.Source.Name, args.Text));
                }
            };

            _standardIrcClient.LocalUser.NoticeReceived +=
                (sender, args) =>
                {
                    _logger.Info(LogTag +
                                 string.Format("[LocalUser.NoticeReceived] {0} {1}", args.Source.Name, args.GetText()));
                };
        }

        private IList<IIrcMessageTarget> GetDefaultReplyTargets(IrcDotNet.IrcClient ircClient, IIrcMessageSource source,
            IList<IIrcMessageTarget> targets)
        {
            if (targets.Contains(ircClient.LocalUser) && source is IIrcMessageTarget)
            {
                return new[] {(IIrcMessageTarget) source};
            }

            return targets;
        }

        protected virtual void OnDccSendReceived(DccSendMessage e)
        {
            var handler = DccSendReceived;
            if (handler != null) handler(this, e);
        }
    }
}