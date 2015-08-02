using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Intel.Haruhichan.ApiClient.Clients;
using Intel.Haruhichan.ApiClient.Models;

namespace Intel.Haruhichan.ApiClient
{
    public class Series
    {
        private readonly IntelHttpClient _intelHttpClient;

        public Series(IntelHttpClient intelHttpClient)
        {
            _intelHttpClient = intelHttpClient;
        }

        public async Task<File[]> Obtain(string searchTerms, Regex regex)
        {
            var search = await _intelHttpClient.Search(searchTerms);
            return null;
        }
    }
}
