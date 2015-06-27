using System;
using System.Collections.Generic;
using Generic.DccClient;
using Generic.IrcClient.Dcc;
using IrcDotNet;

namespace Generic.IrcClient
{
    public class XdccIrcClient
    {
        private readonly IrcUserRegistrationInfo _ircUserRegistrationInfo;
        private readonly StandardIrcClient _standardIrcClient;

        public XdccIrcClient()
        {
            _standardIrcClient = new StandardIrcClient();
            _ircUserRegistrationInfo = new IrcUserRegistrationInfo
            {
                NickName = "speech",
                RealName = "speech",
                UserName = "speech"
            };
        }

        public void Run()
        {
            _standardIrcClient.Connected += (sender, args) =>
            {
                var client = (StandardIrcClient) sender;
                Console.WriteLine("[connected] {0}", client);
            };

            _standardIrcClient.Registered += (sender, args) =>
            {
                var client = (StandardIrcClient) sender;
                client.Channels.Join("#intel");
                RegisterCallbacks();
            };

            _standardIrcClient.Connect("irc.rizon.net", false, _ircUserRegistrationInfo);

            while (true)
            {
            }
        }

        private void RegisterCallbacks()
        {
            _standardIrcClient.LocalUser.JoinedChannel += (sender, args) =>
            {
                var client = (IrcLocalUser) sender;
                var channelEventArgs = args;

                Console.WriteLine("[joined] {0}", channelEventArgs.Channel.Name);
                //client.SendMessage(channelEventArgs.Channel.Name, "Hi!");
                client.SendMessage("CR-HOLLAND|NEW", "xdcc send #3072");

                channelEventArgs.Channel.MessageReceived += (o, eventArgs) =>
                {
                    var c = (IrcChannel) o;
                    //client.SendMessage(GetDefaultReplyTargets(_standardIrcClient, eventArgs.Source, eventArgs.Targets),
                    //    eventArgs.Text);
                    Console.WriteLine("[Channel.MessageReceived] {0} {1}", eventArgs.Source.Name, eventArgs.GetText());
                };

                channelEventArgs.Channel.NoticeReceived += (o, eventArgs) =>
                {
                    var c = (IrcLocalUser) o;
                    //client.SendMessage(GetDefaultReplyTargets(_standardIrcClient, eventArgs.Source, eventArgs.Targets),
                    //    eventArgs.Text);
                    Console.WriteLine("[Channel.NoticeReceived] {0} {1}", eventArgs.Source.Name, eventArgs.GetText());
                };
            };

            _standardIrcClient.LocalUser.MessageReceived += (sender, args) =>
            {
                var client = (IrcLocalUser) sender;

                var dccMessageParser = new DccMessageParser(new IpConverter());

                if (dccMessageParser.IsDccMessage(args.Text))
                {
                    var result = dccMessageParser.Parse(args.Text);

                    var xdccDccClient = new XdccDccClient();
                    xdccDccClient.Download(result.IpAddress, result.Port, result.FileSize, result.FileName);
                }
                else
                {
                    //client.SendMessage(GetDefaultReplyTargets(_standardIrcClient, args.Source, args.Targets), args.Text);
                    Console.WriteLine("[LocalUser.MessageReceived] {0} {1}", args.Source.Name, args.Text);
                }
            };

            _standardIrcClient.LocalUser.NoticeReceived += (sender, args) =>
            {
                var client = (IrcLocalUser) sender;
                //client.SendNotice(GetDefaultReplyTargets(_standardIrcClient, args.Source, args.Targets), args.Text);
                Console.WriteLine("[LocalUser.NoticeReceived] {0} {1}", args.Source.Name, args.GetText());
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
    }
}