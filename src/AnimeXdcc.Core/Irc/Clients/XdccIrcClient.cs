using System;
using System.Linq;
using System.Threading.Tasks;
using IrcDotNet;

namespace AnimeXdcc.Core.Irc.Clients
{
    public class XdccIrcClient : IDisposable
    {
        private readonly StandardIrcClient _standardIrcClient;
        private readonly string _hostname;
        private readonly int _port;

        public XdccIrcClient(string hostname, int port)
        {
            _port = port;
            _hostname = hostname;
            _standardIrcClient = new StandardIrcClient();
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
            bool isRegistered = false;

            _standardIrcClient.Registered += (sender, args) =>
            {
                isRegistered = true;
            };
            
            _standardIrcClient.Connect(_hostname, _port, false, new IrcUserRegistrationInfo
            {
                NickName = "speechlessdownloader",
                Password = null,
                RealName = "speechlessdownloader",
                UserName = "speechlessdownloader"
            });

            while (!isRegistered)
            {
                await Task.Delay(100);
            }
        }

        private async Task<string> FindTargetChannel(string target)
        {
            string channel = null;

            _standardIrcClient.WhoIsReplyReceived += (sender, args) =>
            {
                channel = args.User.Client.Channels.First().Name;
            };

            _standardIrcClient.QueryWhoIs(target);

            while (channel == null)
            {
                await Task.Delay(100);
            }

            return channel;
        }

        private async Task JoinChannel(string channel)
        {
            bool joinedChannel = false;

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
                await Task.Delay(100);
            }
        }

        private async Task RequestPackageTransfer(string target, int packageId)
        {
            var privateMessageSent = false;

            _standardIrcClient.LocalUser.MessageSent += (sender, args) =>
            {
                privateMessageSent = true;
            };

            _standardIrcClient.LocalUser.SendMessage(target, string.Format("XDCC SEND #{0}", packageId));

            while (!privateMessageSent)
            {
                await Task.Delay(100);
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
                await Task.Delay(100);
            }

            return messageReceived;
        }

        public void Dispose()
        {
            if (_standardIrcClient != null)
            {
                _standardIrcClient.Dispose();
            }
        }
    }
}
