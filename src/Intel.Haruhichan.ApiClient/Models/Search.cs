using System.Collections.Generic;
using Newtonsoft.Json;

namespace Intel.Haruhichan.ApiClient.Models
{
    [JsonObject]
    public class Search
    {
        [JsonProperty(PropertyName = "files")]
        public IEnumerable<File> Files { get; set; }

        [JsonProperty(PropertyName = "error")]
        public bool Error { get; set; }
    }
}