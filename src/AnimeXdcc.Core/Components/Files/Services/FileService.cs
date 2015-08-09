using System;
using System.IO;
using AnimeXdcc.Core.Components.Files.Models;

namespace AnimeXdcc.Core.Components.Files.Services
{
    public class FileService : IFileService
    {
        public FileDetails GetDetails(string path)
        {
            FileDetails fileDetails;

            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);

                fileDetails = new FileDetails(
                    fileInfo.Length,
                    fileInfo.IsReadOnly,
                    fileInfo.FullName
                    );
            }
            else
            {
                fileDetails = new FileDetails(
                    0,
                    false,
                    Path.GetFullPath(path)
                    );
            }

            return fileDetails;
        }

        public FileStream GetFileStream(string path, FileStrategy fileStrategy)
        {
            var fileInfo = new FileInfo(path);

            FileStream fileStream;

            switch (fileStrategy)
            {
                case FileStrategy.OverWrite:
                    fileStream = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
                    break;
                case FileStrategy.Resume:
                    fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fileStrategy");
            }

            return fileStream;
        }
    }
}