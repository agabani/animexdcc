using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Logging;
using Intel.Haruhichan.ApiClient.Models;
using Newtonsoft.Json;

namespace Intel.Haruhichan.ApiClient.Clients
{
    public class IntelHttpClient : IIntelHttpClient
    {
        private readonly Uri _baseAddress;
        private readonly ILogger _logger;
        private readonly string _logTag;

        public IntelHttpClient(Uri baseAddress, ILogger logger)
        {
            _baseAddress = baseAddress;
            _logger = logger;
            _logTag = GetType().FullName;
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
            _logger.Debug(_logTag + "[URI] " + relativeUri);
            using (var httpClient = new HttpClient {BaseAddress = _baseAddress})
            {
                return await httpClient.GetAsync(relativeUri, cancellationToken);
            }
        }

        private async Task<Search> Parse(HttpResponseMessage httpResponseMessage)
        {
            var jsonResponseMessage = await httpResponseMessage.Content.ReadAsStringAsync();
            _logger.Debug(_logTag + "[RESPONSE] " + jsonResponseMessage);
            return JsonConvert.DeserializeObject<Search>(jsonResponseMessage);
        }
    }
}