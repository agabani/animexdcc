namespace AnimeXdcc.Core.Tests.Configuration
{
    public class IrcServerConfiguration
    {
        public string HostName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IRC.Server.HostName"]; }
        }

        public int Port
        {
            get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["IRC.Server.Port"]); }
        }
    }
}