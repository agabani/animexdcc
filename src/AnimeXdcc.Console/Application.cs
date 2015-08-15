using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.Logging.Trace;
using Intel.Haruhichan.ApiClient.Clients;
using Intel.Haruhichan.ApiClient.Models;

namespace AnimeXdcc.Console
{
    public class Application : IDisposable
    {
        private readonly IntelHttpClient _intelHttpClient;
        private readonly ConsoleRenderer _renderer;
        private AnimeXdccClient _animeXdccClient;
        private CancellationTokenSource _tokenSource;

        public Application()
        {
            _renderer = new ConsoleRenderer();
            _tokenSource = new CancellationTokenSource();

            _intelHttpClient = new IntelHttpClient(
                new Uri("http://intel.haruhichan.com/"),
                new TraceLogger(TraceLogger.Level.Debug));

            _animeXdccClient = new AnimeXdccClient(
                "irc.rizon.net", 6667,
                new RandomWord().GetString(10));

            _animeXdccClient.TransferStatusEvent += UpdateProgressBar;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task RunAsync()
        {
            _renderer.RenderTitle("AnimeXdcc v0.0.5");
            _renderer.RenderInstructions();
            var searchTerm = System.Console.ReadLine();

            var file = await Search(searchTerm);

            _renderer.RenderFile(file.FileName, file.Size);

            _renderer.DrawProgressBar();

            _renderer.RenderCancel();

            var cancel = Task.Factory.StartNew(() =>
            {
                while (System.Console.ReadKey().Key != ConsoleKey.C)
                {
                }
            });

            var downloadPackageAsync = _animeXdccClient.DownloadPackageAsync(file.BotName, file.PackageNumber,
                _tokenSource.Token);

            var wait = await Task.WhenAny(cancel, downloadPackageAsync);

            if (wait == downloadPackageAsync)
            {
                UpdateProgressBar(null, await downloadPackageAsync);
                _renderer.RenderDownloadComplete();
            }
            else
            {
                _tokenSource.Cancel();
                _renderer.RenderDownloadCancelled();
            }

            _renderer.RenderExit();

            System.Console.ReadKey();
        }

        private async Task<File> Search(string searchTerm)
        {
            var search = await _intelHttpClient.SearchAsync(searchTerm, _tokenSource.Token);

            return search.Files
                .OrderByDescending(r => r.Requested)
                .First();
        }

        private void UpdateProgressBar(object sender, DccTransferStatus dccTransferStatus)
        {
            _renderer.UpdateProgressBar(
                (int) (dccTransferStatus.DownloadedBytes*100/dccTransferStatus.FileSize),
                dccTransferStatus.FileSize,
                (long) dccTransferStatus.BytesPerMillisecond*1024,
                (dccTransferStatus.FileSize - dccTransferStatus.DownloadedBytes)/
                (dccTransferStatus.BytesPerMillisecond*1024)
                );
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_animeXdccClient != null)
                {
                    _animeXdccClient.Dispose();
                    _animeXdccClient = null;
                }

                if (_tokenSource != null)
                {
                    _tokenSource.Dispose();
                    _tokenSource = null;
                }
            }
        }

        ~Application()
        {
            Dispose(false);
        }
    }
}