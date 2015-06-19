using System;
using System.Net.Http;
using System.Threading.Tasks;
using Intel.Haruhichan.Characterization.Tests.Models;
using Newtonsoft.Json;

namespace Intel.Haruhichan.Characterization.Tests.Core
{
    public class IntelHttpClient
    {
        private readonly Uri _baseAddress;

        public IntelHttpClient(Uri baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public async Task<Search> Search(string term)
        {
            var requestUri = string.Format("ajax.php?a=s&t={0}", term);
            return await ExecuteQuery(requestUri);
        }

        public async Task<Search> Bot(int id)
        {
            var requestUri = string.Format("ajax.php?a=b&id={0}", id);
            return await ExecuteQuery(requestUri);
        }

        private async Task<Search> ExecuteQuery(string requestUri)
        {
            using (var httpResponseMessage = await Get(requestUri))
            {
                return await Parse(httpResponseMessage);
            }
        }

        private async Task<HttpResponseMessage> Get(string relativeUri)
        {
            using (var httpClient = new HttpClient {BaseAddress = _baseAddress})
            {
                return await httpClient.GetAsync(relativeUri);
            }
        }

        private static async Task<Search> Parse(HttpResponseMessage httpResponseMessage)
        {
            var jsonResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Search>(jsonResponseMessage);
        }
    }
}