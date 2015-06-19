using System;

namespace Intel.Haruhichan.Characterization.Tests.Core
{
    public class ConfigurationManager
    {
        public Uri BaseUri
        {
            get { return new Uri(System.Configuration.ConfigurationManager.AppSettings["BaseUri"]); }
        }
    }
}