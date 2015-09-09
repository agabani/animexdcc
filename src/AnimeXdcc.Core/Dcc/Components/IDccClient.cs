using System.IO;
using System.Threading.Tasks;

namespace AnimeXdcc.Core.Dcc.Components
{
    public interface IDccClient
    {
        Task DownloadAsync(string hostname, int port, long size, Stream stream);
        event DccClient.TransferProgressEventHandler TransferProgress;
    }
}