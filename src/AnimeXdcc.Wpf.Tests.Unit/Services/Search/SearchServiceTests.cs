using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AnimeXdcc.Wpf.Models;
using AnimeXdcc.Wpf.Services.Search;
using Moq;
using NUnit.Framework;

namespace AnimeXdcc.Wpf.Tests.Unit.Services.Search
{
    [TestFixture]
    public class SearchServiceTests
    {
        [Test]
        public void Should_not_be_able_to_initalize_without_sources()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() => new SearchService(null));
        }

        [Test]
        public async Task Should_be_able_to_invoke_all_sources()
        {
            var mock1 = new Mock<ISearchable>();
            var mock2 = new Mock<ISearchable>();
            var mock3 = new Mock<ISearchable>();

            mock1
                .Setup(m => m.SearchAsync("term", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>());

            mock2
                .Setup(m => m.SearchAsync("term", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>());

            mock3
                .Setup(m => m.SearchAsync("term", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>());

            var searchables = new List<ISearchable>
            {
                mock1.Object,
                mock2.Object,
                mock3.Object
            };

            await new SearchService(searchables).SearchAsync("term");

            mock1.Verify(m => m.SearchAsync("term", It.IsAny<CancellationToken>()), Times.Once());
            mock2.Verify(m => m.SearchAsync("term", It.IsAny<CancellationToken>()), Times.Once());
            mock3.Verify(m => m.SearchAsync("term", It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task Should_be_able_to_group_all_packages()
        {
            var mock1 = new Mock<ISearchable>();
            var mock2 = new Mock<ISearchable>();
            var mock3 = new Mock<ISearchable>();
            var mock4 = new Mock<ISearchable>();
            var mock5 = new Mock<ISearchable>();

            mock1
                .Setup(m => m.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>
                {
                    new DccPackage("File 1", "Bot A", 1, "100M"),
                    new DccPackage("File 2", "Bot B", 2, "200M"),
                    new DccPackage("File 3", "Bot C", 3, "300M"),
                    new DccPackage("File 4", "Bot D", 4, "400M"),
                    new DccPackage("File 5", "Bot E", 5, "500M")
                });

            mock2
                .Setup(m => m.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>
                {
                    new DccPackage("File_1", "Bot A", 1, "100M"),
                    new DccPackage("File_2", "Bot B", 2, "200M"),
                    new DccPackage("File_3", "Bot C", 3, "300M"),
                    new DccPackage("File_4", "Bot D", 4, "400M"),
                    new DccPackage("File_5", "Bot E", 5, "500M")
                });

            mock3
                .Setup(m => m.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>
                {
                    new DccPackage("File 1\r", "Bot A", 1, "100M"),
                    new DccPackage("File 2\r", "Bot B", 2, "200M"),
                    new DccPackage("File 3\r", "Bot C", 3, "300M"),
                    new DccPackage("File 4\r", "Bot D", 4, "400M"),
                    new DccPackage("File 5\r", "Bot E", 5, "500M")
                });

            mock4
                .Setup(m => m.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>
                {
                    new DccPackage("file 1", "Bot A", 1, "100M"),
                    new DccPackage("file 2", "Bot B", 2, "200M"),
                    new DccPackage("file 3", "Bot C", 3, "300M"),
                    new DccPackage("file 4", "Bot D", 4, "400M"),
                    new DccPackage("file 5", "Bot E", 5, "500M")
                });

            mock4
                .Setup(m => m.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>
                {
                    new DccPackage("file 1", "Bot A", 1, "100M"),
                    new DccPackage("file 2", "Bot B", 2, "200M"),
                    new DccPackage("file 3", "Bot C", 3, "300M"),
                    new DccPackage("file 4", "Bot D", 4, "400M"),
                    new DccPackage("file 5", "Bot E", 5, "500M")
                });

            mock5
                .Setup(m => m.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DccPackage>
                {
                    new DccPackage("file 1", "Bot A", 1, "100M"),
                    new DccPackage("file 2", "Bot B", 22, "200M"),
                    new DccPackage("file 3", "Bot CC", 3, "300M"),
                    new DccPackage("file 4", "Bot DD", 44, "400M"),
                    new DccPackage("file 5", "Bot E", 5, "500M")
                });

            var searchables = new List<ISearchable>
            {
                mock1.Object,
                mock2.Object,
                mock3.Object,
                mock4.Object,
                mock5.Object
            };

            var result = await new SearchService(searchables).SearchAsync("term");

            Assert.That(result.Count, Is.EqualTo(5));

            Assert.That(result.First(s => s.FileName == "File 1").DccPackages.Count, Is.EqualTo(1));
            Assert.That(result.First(s => s.FileName == "File 2").DccPackages.Count, Is.EqualTo(2));
            Assert.That(result.First(s => s.FileName == "File 3").DccPackages.Count, Is.EqualTo(2));
            Assert.That(result.First(s => s.FileName == "File 4").DccPackages.Count, Is.EqualTo(2));
            Assert.That(result.First(s => s.FileName == "File 5").DccPackages.Count, Is.EqualTo(1));
        }
    }
}
