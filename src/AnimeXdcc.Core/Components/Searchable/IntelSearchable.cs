﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Clients.Models;
using Intel.Haruhichan.ApiClient.Clients;

namespace AnimeXdcc.Core.Components.Searchable
{
    public class IntelSearchable : ISearchable
    {
        private readonly IIntelHttpClient _intel;

        public IntelSearchable(IIntelHttpClient intel)
        {
            _intel = intel;
        }

        public async Task<List<DccPackage>> SearchAsync(string searchTerm, CancellationToken token = new CancellationToken())
        {
            var search = await _intel.SearchAsync(searchTerm, token);

            return search.Files.Select(p => new DccPackage(p.FileName, p.BotName, p.PackageNumber, p.Size, p.Requested)).ToList();
        }
    }
}