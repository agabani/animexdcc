using System;
using System.IO;

namespace AnimeXdcc.Wpf.Services.Download
{
    public interface IStreamProvider
    {
        Stream GetStream(string fileName);
    }

    public class StreamProvider : IStreamProvider
    {
        private Uri _folderUri;

        public StreamProvider(string folderPath)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            _folderUri = new Uri(baseDirectory + "/" + folderPath);
        }

        public Stream GetStream(string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}