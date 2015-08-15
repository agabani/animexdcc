using AnimeXdcc.Core.Components.Filters.Builders;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Unit.Components.Filters.Builders
{
    [TestFixture]
    public class RegexPatternBuilderTests
    {
        [Test]
        public void Static_with_episode_number()
        {
            var pattern = new RegexPatternBuilder()
                .WithStatic("[HorribleSubs] Kuroko's Basketball 3 - ")
                .CaptureEpisode()
                .WithStatic(" [1080p].mkv")
                .Build();

            Assert.That(pattern, Is.EqualTo(@"\[HorribleSubs\] Kuroko's Basketball 3 - (\d+) \[1080p\].mkv"));
        }

        [Test]
        public void Static_with_episode_number_and_crc()
        {
            var pattern = new RegexPatternBuilder()
                .WithStatic("[Commie] LogEvent Horizon - ")
                .CaptureEpisode()
                .WithStatic(" [")
                .CaptureCrc()
                .WithStatic("].mkv")
                .Build();

            Assert.That(pattern, Is.EqualTo(@"\[Commie\] LogEvent Horizon - (\d+) \[([A-F0-9]{8})\].mkv"));
        }

        [Test]
        public void Static_with_episode_number_and_version()
        {
            var pattern = new RegexPatternBuilder()
                .WithStatic("[HorribleSubs] Fate Stay Night - Unlimited Blade Works - ")
                .CaptureEpisode()
                .CaptureVersion()
                .WithStatic(" [1080p].mkv")
                .Build();

            Assert.That(pattern,
                Is.EqualTo(@"\[HorribleSubs\] Fate Stay Night - Unlimited Blade Works - (\d+)v?(\d+)? \[1080p\].mkv"));
        }
    }
}