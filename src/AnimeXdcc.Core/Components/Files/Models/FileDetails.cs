namespace AnimeXdcc.Core.Components.Files.Models
{
    public class FileDetails
    {
        public FileDetails(long length, bool isReadOnly, string fullPath)
        {
            Length = length;
            IsReadOnly = isReadOnly;
            FullPath = fullPath;
        }

        public long Length { get; private set; }
        public bool IsReadOnly { get; private set; }
        public string FullPath { get; private set; }
    }
}