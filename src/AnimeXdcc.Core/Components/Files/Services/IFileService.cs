using System.IO;
using AnimeXdcc.Core.Components.Files.Models;

namespace AnimeXdcc.Core.Components.Files.Services
{
    public interface IFileService
    {
        FileDetails GetDetails(string path);
        FileStream GetFileStream(string path, FileStrategy fileStrategy);
    }
}