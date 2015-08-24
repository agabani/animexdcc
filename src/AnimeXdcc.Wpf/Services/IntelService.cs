using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intel.Haruhichan.ApiClient.Clients;

namespace AnimeXdcc.Wpf.Services
{
    internal class IntelService : IIntelService
    {
        private readonly IIntelHttpClient _intelHttpClient;

        public IntelService(IIntelHttpClient intelHttpClient)
        {
            _intelHttpClient = intelHttpClient;
        }

        public async Task<List<Package>> SearchAsync(string searchTerm)
        {
            var search = await _intelHttpClient.SearchAsync(searchTerm);

            var groups = search.Files.GroupBy(f => f.FileName.Replace("\r", "").ToUpper());

            var result = new List<Package>();

            foreach (var grouping in groups)
            {
                var file = grouping.OrderByDescending(f => f.Requested).First();
                result.Add(new Package
                {
                    FileName = file.FileName.Replace("\r", "")
                });
            }

            return result;
        }
    }

    internal class Package
    {
        public string FileName { get; set; }
    }
}