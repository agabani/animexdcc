using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Files;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public interface IDownloadClient
    {
        Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package, IStreamProvider provider);
    }
}