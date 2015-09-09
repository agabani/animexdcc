using System.IO;

namespace AnimeXdcc.Core.Components.Files
{
    public interface IStreamProvider
    {
        Stream GetStream(string fileName, StreamProvider.Strategy strategy);
    }
}