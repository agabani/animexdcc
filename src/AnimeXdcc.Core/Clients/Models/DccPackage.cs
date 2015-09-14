namespace AnimeXdcc.Core.Clients.Models
{
    public class DccPackage
    {
        public DccPackage(string fileName, string botName, int packageId, string fileSize, int requested)
        {
            FileName = fileName;
            BotName = botName;
            PackageId = packageId;
            FileSize = fileSize;
            Requested = requested;
        }

        public string FileName { get; private set; }
        public string BotName { get; private set; }
        public int PackageId { get; private set; }
        public string FileSize { get; private set; }
        public int Requested { get; private set; }
    }
}