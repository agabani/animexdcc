using Newtonsoft.Json;

namespace Intel.Haruhichan.Characterization.Tests.Models
{
    [JsonObject]
    public class File
    {
        [JsonProperty(PropertyName = "botid")]
        public int BotId { get; set; }

        [JsonProperty(PropertyName = "botname")]
        public string BotName { get; set; }

        [JsonProperty(PropertyName = "filename")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "packnum")]
        public int PackageNumber { get; set; }

        [JsonProperty(PropertyName = "requestid")]
        public int Requested { get; set; }

        [JsonProperty(PropertyName = "size")]
        public string Size { get; set; }
    }
}