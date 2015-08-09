using AnimeXdcc.Core.Components.FilesManager;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Components.FilesManager
{
    [TestFixture]
    public class FilesManagerServiceTests
    {
        [Test]
        public void Should_sort_series_into_folders()
        {
            var service = new FilesManagerService(@"c:\folder");

            var path = service.GetFilePath("series", "[group] series - 01 [resolution].extension");

            Assert.That(path, Is.EqualTo(@"c:\folder\series\[group] series - 01 [resolution].extension"));
        }
    }
}
