using AnimeXdcc.Core.Components.Filters.Builders;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Unit.Components.Filters.Builders
{
    [TestFixture]
    public class SearchTermBuilderTests
    {
        [Test]
        public void Static_with_episode_number()
        {
            var pattern = new SearchTermBuilder()
                .WithStatic("[HorribleSubs] Kuroko's Basketball 3 - ")
                .CaptureEpisode()
                .WithStatic(" [1080p].mkv")
                .Build();

            Assert.That(pattern, Is.EqualTo("[HorribleSubs] Kuroko's Basketball 3 - {0} [1080p].mkv"));
        }

        [Test]
        public void Static_with_episode_number_and_crc()
        {
            var pattern = new SearchTermBuilder()
                .WithStatic("[Commie] LogEvent Horizon - ")
                .CaptureEpisode()
                .WithStatic(" [")
                .CaptureCrc()
                .WithStatic("].mkv")
                .Build();

            Assert.That(pattern, Is.EqualTo(@"[Commie] LogEvent Horizon - {0} [ ].mkv"));
        }

        [Test]
        public void Static_with_episode_number_and_version()
        {
            var pattern = new SearchTermBuilder()
                .WithStatic("[HorribleSubs] Fate Stay Night - Unlimited Blade Works - ")
                .CaptureEpisode()
                .CaptureVersion()
                .WithStatic(" [1080p].mkv")
                .Build();

            Assert.That(pattern,
                Is.EqualTo(@"[HorribleSubs] Fate Stay Night - Unlimited Blade Works - {0}v{1} [1080p].mkv"));
        }
    }
}
