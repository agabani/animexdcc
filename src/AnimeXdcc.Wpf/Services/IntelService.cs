using Intel.Haruhichan.ApiClient.Clients;

namespace AnimeXdcc.Wpf.Services
{
    internal class IntelService : IIntelService
    {
        private IIntelHttpClient _intelHttpClient;

        public IntelService(IIntelHttpClient intelHttpClient)
        {
            _intelHttpClient = intelHttpClient;
        }
    }
}