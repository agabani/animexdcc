namespace AnimeXdcc.Client.Console.Configuration
{
    public class XdccConfiguration
    {
        public string BaseUrl
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["XDCC.SearchEngine.BaseUrl"]; }
        }

        public string SearchTerm
        {
            get { return System.Configuration.ConfigurationManager.AppSettings["XDCC.SearchEngine.SearchTerm"]; }
        }
    }
}