namespace AnimeXdcc.Core.Tests.Unit.Configuration
{
    public class ConfigurationManager
    {
        public ConfigurationManager()
        {
            Irc = new IrcConfiguration();
            Xdcc = new XdccConfiguration();
        }

        public IrcConfiguration Irc { get; private set; }
        public XdccConfiguration Xdcc { get; private set; }
    }
}