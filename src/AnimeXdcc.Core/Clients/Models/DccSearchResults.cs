using System.Collections.Generic;

namespace AnimeXdcc.Core.Clients.Models
{
    public class DccSearchResults
    {
        public DccSearchResults(string fileName, string fileSize, List<DccPackage> dccPackages)
        {
            FileName = fileName;
            FileSize = fileSize;
            DccPackages = dccPackages;
        }

        public string FileName { get; private set; }
        public string FileSize { get; private set; }
        public List<DccPackage> DccPackages { get; private set; }
    }
}