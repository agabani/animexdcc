using System;
using System.IO;
using System.Security.AccessControl;

namespace AnimeXdcc.Core.Components.Files
{
    public class StreamProvider : IStreamProvider
    {
        public enum Strategy
        {
            Overwrite,
            Resume
        }

        private readonly Uri _folderUri;

        public StreamProvider()
            : this(AppDomain.CurrentDomain.BaseDirectory, string.Empty)
        {
        }

        public StreamProvider(string folderPath)
            : this(AppDomain.CurrentDomain.BaseDirectory, folderPath)
        {
        }

        public StreamProvider(string baseDirectory, string folderPath)
        {
            _folderUri = new Uri(string.Format("{0}/{1}/", baseDirectory, folderPath));
        }

        public Stream GetStream(string fileName, Strategy strategy)
        {
            var uri = new Uri(_folderUri, fileName);

            switch (strategy)
            {
                case Strategy.Overwrite:
                    return new FileStream(uri.LocalPath, FileMode.Create);
                case Strategy.Resume:
                    return new FileStream(uri.LocalPath, FileMode.Append);
                default:
                    throw new ArgumentOutOfRangeException("strategy");
            }
        }
    }
}