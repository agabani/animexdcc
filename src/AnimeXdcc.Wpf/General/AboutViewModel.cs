using System.Diagnostics;
using AnimeXdcc.Wpf.Infrastructure.Bindable;
using AnimeXdcc.Wpf.Infrastructure.Relay;

namespace AnimeXdcc.Wpf.General
{
    internal class AboutViewModel : BindableBase
    {
        public RelayCommand WebsiteCommand { get; private set; }

        public AboutViewModel()
        {
            WebsiteCommand = new RelayCommand(LaunchWebsite);
        }

        private void LaunchWebsite()
        {
            Process.Start("http://www.animexdcc.com");
        }
    }
}