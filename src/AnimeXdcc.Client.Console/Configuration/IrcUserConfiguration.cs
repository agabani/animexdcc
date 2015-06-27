namespace AnimeXdcc.Client.Console.Configuration
{
    public class IrcUserConfiguration
    {
        public string NickName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IRC.User.NickName"]; }
        }

        public string RealName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IRC.User.RealName"]; }
        }

        public string UserName
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["IRC.User.UserName"]; }
        }
    }
}