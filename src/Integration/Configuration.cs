using System.Configuration;

namespace Integration
{
    public class Configuration
    {
        private readonly IrcServerConfiguration _ircServer;

        public Configuration()
        {
            _ircServer = new IrcServerConfiguration();
        }

        public IrcServerConfiguration IrcServer
        {
            get { return _ircServer; }
        }

        public class IrcServerConfiguration
        {
            public string HostName
            {
                get { return ConfigurationManager.AppSettings["irc.server.hostname"]; }
            }

            public int Port
            {
                get { return int.Parse(ConfigurationManager.AppSettings["irc.server.port"]); }
            }
        }
    }
}