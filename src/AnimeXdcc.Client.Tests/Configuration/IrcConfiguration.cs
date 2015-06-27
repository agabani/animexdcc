namespace AnimeXdcc.Client.Tests.Configuration
{
    public class IrcConfiguration
    {
        public IrcConfiguration()
        {
            Server = new IrcServerConfiguration();
            User = new IrcUserConfiguration();
        }

        public IrcServerConfiguration Server { get; private set; }
        public IrcUserConfiguration User { get; private set; }

        public string Channel
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IRC.Channel"]; }
        }
    }
}