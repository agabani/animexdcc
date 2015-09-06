namespace AnimeXdcc.Wpf.Models
{
    public class DccPackage
    {
        public DccPackage(string fileName, string botName, int packageId, string fileSize)
        {
            FileName = fileName;
            BotName = botName;
            PackageId = packageId;
            FileSize = fileSize;
        }

        public string FileName { get; private set; }
        public string BotName { get; private set; }
        public int PackageId { get; private set; }
        public string FileSize { get; private set; }
    }
}