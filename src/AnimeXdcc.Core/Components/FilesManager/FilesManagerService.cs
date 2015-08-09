using System.IO;

namespace AnimeXdcc.Core.Components.FilesManager
{
    public class FilesManagerService : IFilesManagerService
    {
        private readonly string _baseDirectory;

        public FilesManagerService(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public string GetFilePath(string series, string fileName)
        {
            var directory = Path.Combine(_baseDirectory, series);
            var filePath = Path.Combine(directory, fileName);
            return Path.GetFullPath(filePath);
        }
    }
}