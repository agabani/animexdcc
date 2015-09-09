using System.Threading.Tasks;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Download
{
    public interface IDownloadService
    {
        Task<DownloadClient.DownloadResult> DownloadAsync(DccPackage package);
    }
}