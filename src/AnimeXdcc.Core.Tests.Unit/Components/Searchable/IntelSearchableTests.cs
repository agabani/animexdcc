using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Core.Components.Searchable;
using Intel.Haruhichan.ApiClient.Clients;
using Intel.Haruhichan.ApiClient.Models;
using Moq;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Unit.Components.Searchable
{
    [TestFixture]
    public class IntelSearchableTests
    {
        [Test]
        public async Task Should_invoke_search_service()
        {
            var mock = new Mock<IIntelHttpClient>();

            mock
                .Setup(m => m.SearchAsync("term", new CancellationToken()))
                .ReturnsAsync(
                    new Search
                    {
                        Error = false,
                        Files = new List<File>
                        {
                            new File
                            {
                                BotId = 1,
                                BotName = "bot name",
                                FileName = "file name",
                                PackageNumber = 1,
                                Requested = 1,
                                Size = "1M"
                            },
                            new File
                            {
                                BotId = 2,
                                BotName = "bot name",
                                FileName = "file name",
                                PackageNumber = 2,
                                Requested = 2,
                                Size = "2M"
                            },
                            new File
                            {
                                BotId = 3,
                                BotName = "bot name",
                                FileName = "file name",
                                PackageNumber = 3,
                                Requested = 3,
                                Size = "3M"
                            }
                        }
                    }
                );

            var packages = await new IntelSearchable(mock.Object).SearchAsync("term");

            mock
                .Verify(m => m.SearchAsync("term", It.IsAny<CancellationToken>()));

            Assert.That(packages.Count, Is.EqualTo(3));
            Assert.That(packages.Any(b => b.PackageId == 1));
            Assert.That(packages.Any(b => b.PackageId == 2));
            Assert.That(packages.Any(b => b.PackageId == 3));
        }

        [Test]
        public async Task Should_map_intel_results_to_generic_results()
        {
            var mock = new Mock<IIntelHttpClient>();

            mock
                .Setup(m => m.SearchAsync("term", new CancellationToken()))
                .ReturnsAsync(
                    new Search
                    {
                        Error = false,
                        Files = new List<File>
                        {
                            new File
                            {
                                BotId = 1,
                                BotName = "bot name",
                                FileName = "file name",
                                PackageNumber = 2,
                                Requested = 3,
                                Size = "1M"
                            }
                        }
                    }
                );

            var package = (await new IntelSearchable(mock.Object).SearchAsync("term")).First();

            Assert.That(package.BotName, Is.EqualTo("bot name"));
            Assert.That(package.FileName, Is.EqualTo("file name"));
            Assert.That(package.FileSize, Is.EqualTo("1M"));
            Assert.That(package.PackageId, Is.EqualTo(2));
            Assert.That(package.Requested, Is.EqualTo(3));
        }
    }
}