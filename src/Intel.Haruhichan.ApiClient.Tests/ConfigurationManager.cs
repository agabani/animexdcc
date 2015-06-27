using System;

namespace Intel.Haruhichan.ApiClient.Tests
{
    public class ConfigurationManager
    {
        public Uri BaseUri
        {
            get { return new Uri(System.Configuration.ConfigurationManager.AppSettings["BaseUri"]); }
        }
    }
}