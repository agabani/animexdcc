using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Wpf.Models;

namespace AnimeXdcc.Wpf.Services.Search
{
    public class SearchService
    {
        private readonly List<ISearchable> _searchables;

        public SearchService(List<ISearchable> searchables)
        {
            if (searchables == null)
            {
                throw new ArgumentNullException("searchables");
            }

            _searchables = searchables;
        }

        public async Task<List<DccSearchResults>> SearchAsync(string searchTerm,
            CancellationToken token = default(CancellationToken))
        {
            var packages = await SearchAllSearchables(searchTerm, token);

            var results = GroupResults(packages);

            return results;
        }

        private async Task<List<DccPackage>> SearchAllSearchables(string searchTerm, CancellationToken token)
        {
            var packages = new List<DccPackage>();

            foreach (var service in _searchables)
            {
                packages.AddRange(await service.SearchAsync(searchTerm, token));
            }

            return packages;
        }

        private static List<DccSearchResults> GroupResults(IEnumerable<DccPackage> packages)
        {
            return packages
                .GroupBy(f => Uniform(f.FileName))
                .Select(g =>
                    new DccSearchResults(
                        Sanitize(g.First().FileName),
                        g.First().FileSize,
                        g.GroupBy(p => new {p.BotName, p.PackageId}).Select(p => p.First()).ToList()
                        )
                ).ToList();
        }

        private static string Sanitize(string text)
        {
            return text
                .Replace("\r", "")
                .Replace("_", " ");
        }

        private static string Uniform(string text)
        {
            return Sanitize(text).ToUpper();
        }
    }

    public class DccSearchResults
    {
        public DccSearchResults(string fileName, string fileSize, List<DccPackage> dccPackages)
        {
            FileName = fileName;
            FileSize = fileSize;
            DccPackages = dccPackages;
        }

        public string FileName { get; private set; }
        public string FileSize { get; private set; }
        public List<DccPackage> DccPackages { get; private set; }
    }
}