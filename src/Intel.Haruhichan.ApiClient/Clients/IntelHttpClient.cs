using System;
using System.Net.Http;
using System.Threading.Tasks;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Models;
using Newtonsoft.Json;

namespace Intel.Haruhichan.ApiClient.Clients
{
    public class IntelHttpClient
    {
        private const string LogTag = "[IntelHttpClient] ";
        private readonly Uri _baseAddress;
        private readonly ILogger _logger;

        public IntelHttpClient(Uri baseAddress, ILogger logger)
        {
            _baseAddress = baseAddress;
            _logger = logger;
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
            _logger.Debug(LogTag + "[URI] " + relativeUri);
            using (var httpClient = new HttpClient {BaseAddress = _baseAddress})
            {
                return await httpClient.GetAsync(relativeUri);
            }
        }

        private async Task<Search> Parse(HttpResponseMessage httpResponseMessage)
        {
            var jsonResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            _logger.Debug(LogTag + "[RESPONSE] " + jsonResponseMessage);
            return JsonConvert.DeserializeObject<Search>(jsonResponseMessage);
        }
    }
}