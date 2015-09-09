using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.Irc.Clients;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public class DownloadService
    {
        private readonly IDownloadClient _client;

        public DownloadService(IDownloadClient client)
        {
            _client = client;
        }

        public async Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package)
        {
            await _client.DownloadAsync(package);
            throw new NotImplementedException();
        }
    }
}
