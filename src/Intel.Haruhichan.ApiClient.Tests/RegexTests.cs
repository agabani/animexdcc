using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Intel.Haruhichan.ApiClient.Tests
{
    [TestFixture]
    public class RegexTests
    {
        [Test]
        public void Should_be_able_to_identify_name()
        {
            const string name = "[Doki] Angel Beats! - 01 (1280x720 Hi10P BD AAC) [74EAE552].mkv";

            var regex = new Regex(@"\[Doki\] Angel Beats! - \d+ \(1280x720 Hi10p BD AAC\) \[[0-9A-F]{8}\].mkv", RegexOptions.IgnoreCase);

            Assert.That(regex.IsMatch(name), Is.True);

            var result = Regex.Match(name, @"\d+").Value;

            Assert.That(result, Is.EqualTo("01"));

            var values =
                Regex.Match(name, @"(?<=\[Doki\] Angel Beats! - )\d+(?= \(1280x720 Hi10p BD AAC\) \[[0-9A-F]{8}\].mkv)");

 
            foreach (var capture in values.Captures)
            {
                Console.WriteLine(capture);
            }

            Console.WriteLine(values);

            //Assert.That(values, Is.EqualTo("01"));
        }

        [Test]
        public void CaptureValues()
        {
            const string input = "AddedProject[12_35_55_219]0";
            string part = Regex.Match(input, @"\[[\d_]+\]").Captures[0].Value;

            Assert.That(part, Is.EqualTo("[12_35_55_219]"));
        }
    }
}
