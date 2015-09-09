using System;
using System.IO;
using AnimeXdcc.Core.Components.Files;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Integration.Components.Files
{
    [TestFixture]
    public class StreamProviderTests
    {
        [Test]
        public void Should_create_file_at_exe_location()
        {
            new StreamProvider().GetStream("my file", StreamProvider.Strategy.Overwrite).Dispose();

            File.Exists(string.Format("{0}/my file", AppDomain.CurrentDomain.BaseDirectory));

            Assert.That(File.Exists(string.Format("{0}/my file", AppDomain.CurrentDomain.BaseDirectory)), Is.True);
        }

        [Test]
        public void Should_create_file_at_relative_location()
        {
            if (!Directory.Exists("relative folder"))
            {
                Directory.CreateDirectory("relative folder");
            }

            new StreamProvider("relative folder").GetStream("my file", StreamProvider.Strategy.Overwrite).Dispose();

            Assert.That(
                File.Exists(string.Format("{0}/relative folder/my file", AppDomain.CurrentDomain.BaseDirectory)),
                Is.True);
        }

        [Test]
        public void Should_create_file_at_absolute_location()
        {
            if (!Directory.Exists("absolute folder"))
            {
                Directory.CreateDirectory("absolute folder");
            }

            new StreamProvider(AppDomain.CurrentDomain.BaseDirectory, "absolute folder")
                .GetStream("my file", StreamProvider.Strategy.Overwrite).Dispose();

            Assert.That(
                File.Exists(string.Format("{0}/absolute folder/my file", AppDomain.CurrentDomain.BaseDirectory)),
                Is.True);
        }
    }
}