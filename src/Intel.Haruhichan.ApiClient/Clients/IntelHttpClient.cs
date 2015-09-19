using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Intel.Haruhichan.ApiClient.Models;
using Newtonsoft.Json;

namespace Intel.Haruhichan.ApiClient.Clients
{
    // TODO: search methods should be able to give feedback on errors such as "cannot connect to host"
    public class IntelHttpClient : IIntelHttpClient
    {
        private readonly Uri _baseAddress;

        public IntelHttpClient(Uri baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public async Task<Search> SearchAsync(string term, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestUri = string.Format("ajax.php?a=s&t={0}", term);
            return await ExecuteQuery(requestUri, cancellationToken);
        }

        public async Task<Search> BotAsync(int id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestUri = string.Format("ajax.php?a=b&id={0}", id);
            return await ExecuteQuery(requestUri, cancellationToken);
        }

        private async Task<Search> ExecuteQuery(string requestUri, CancellationToken cancellationToken)
        {
            using (var httpResponseMessage = await Get(requestUri, cancellationToken))
            {
                return await Parse(httpResponseMessage);
            }
        }

        private async Task<HttpResponseMessage> Get(string relativeUri, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient {BaseAddress = _baseAddress})
            {
                return await httpClient.GetAsync(relativeUri, cancellationToken);
            }
        }

        private async Task<Search> Parse(HttpResponseMessage httpResponseMessage)
        {
            var jsonResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Search>(jsonResponseMessage);
        }
    }
}