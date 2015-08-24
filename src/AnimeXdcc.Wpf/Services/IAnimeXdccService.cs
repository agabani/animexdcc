using System;
using System.Threading.Tasks;

namespace AnimeXdcc.Wpf.Services
{
    internal interface IAnimeXdccService
    {
        Task DownloadAsync(string bot, int packageId);
        event EventHandler<DownloadProgress> DownloadProgressEvent;
    }
}