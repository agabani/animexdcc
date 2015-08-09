using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Experiments
{
    [TestFixture]
    public class RegexPatternsTests
    {
        [Test]
        [TestCase(@"[HorribleSubs] Fate Stay Night - Unlimited Blade Works - 01v2 [1080p].mkv", "01", "2")]
        [TestCase(@"[HorribleSubs] Fate Stay Night - Unlimited Blade Works - 11 [1080p].mkv", "11", null)]
        public void HorribleSubs_Fate_Stay_Night_Unlimited_Blade_Works(string input, string episode, string version)
        {
            var match = Regex.Match(input,
                @"\[HorribleSubs\] Fate Stay Night - Unlimited Blade Works - (\d+)v?(\d+)? \[1080p\].mkv",
                RegexOptions.IgnoreCase);

            Assert.That(match.Success, Is.True);

            foreach (var @group in match.Groups)
            {
                Console.WriteLine(@group);
            }

            Assert.That(match.Groups.Count, Is.EqualTo(2).Or.EqualTo(3));
            Assert.That(match.Groups[1].Value, Is.EqualTo(episode));
            if (version != null)
            {
                Assert.That(match.Groups[2].Value, Is.EqualTo(version));
            }
        }

        [Test]
        [TestCase(@"[HorribleSubs] Kuroko's Basketball 3 - 51 [1080p].mkv", "51")]
        [TestCase(@"[HorribleSubs] Kuroko's Basketball 3 - 75 [1080p].mkv", "75")]
        public void HorribleSubs_Kurokos_Basketball_3(string input, string episode)
        {
            var match = Regex.Match(input, @"\[HorribleSubs\] Kuroko's Basketball 3 - (\d+) \[1080p\].mkv",
                RegexOptions.IgnoreCase);

            Assert.That(match.Success, Is.True);

            foreach (var @group in match.Groups)
            {
                Console.WriteLine(@group);
            }

            Assert.That(match.Groups.Count, Is.EqualTo(2));
            Assert.That(match.Groups[1].Value, Is.EqualTo(episode));
        }

        [Test]
        [TestCase(@"[Commie] Log Horizon - 06 [53AF6698].mkv", "06", "53AF6698")]
        [TestCase(@"[Commie] Log Horizon - 21 [62388659].mkv", "21", "62388659")]
        public void Commie_Log_Horizon(string input, string episode, string crc)
        {
            var match = Regex.Match(input, @"\[Commie\] Log Horizon - (\d+) \[([A-F0-9]{8})\].mkv",
                RegexOptions.IgnoreCase);

            Assert.That(match.Success, Is.True);

            foreach (var @group in match.Groups)
            {
                Console.WriteLine(@group);
            }

            Assert.That(match.Groups.Count, Is.EqualTo(3));
            Assert.That(match.Groups[1].Value, Is.EqualTo(episode));
            Assert.That(match.Groups[2].Value, Is.EqualTo(crc));
        }
    }
}