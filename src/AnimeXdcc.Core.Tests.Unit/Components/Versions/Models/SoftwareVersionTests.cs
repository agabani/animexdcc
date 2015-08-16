using System;
using AnimeXdcc.Core.Components.Versions.Models;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Unit.Components.Versions.Models
{
    [TestFixture]
    public class SoftwareVersionTests
    {
        [Test]
        public void Should_be_able_to_try_parse_valid_version_string()
        {
            SoftwareVersion softwareVersion;
            var success = SoftwareVersion.TryParse("5.17.2001", out softwareVersion);
            Assert.That(success, Is.True);
            Assert.That(softwareVersion.Major, Is.EqualTo(5));
            Assert.That(softwareVersion.Minor, Is.EqualTo(17));
            Assert.That(softwareVersion.Patch, Is.EqualTo(2001));
        }

        [Test]
        public void Should_not_be_able_to_try_parse_invalid_version_string()
        {
            SoftwareVersion softwareVersion;
            var success = SoftwareVersion.TryParse("5a.14.213", out softwareVersion);
            Assert.That(success, Is.False);
            Assert.That(softwareVersion, Is.Null);
        }

        [Test]
        public void Should_be_able_to_parse_valid_version_string()
        {
            SoftwareVersion softwareVersion = SoftwareVersion.Parse("15.3.6");
            Assert.That(softwareVersion, Is.Not.Null);
            Assert.That(softwareVersion.Major, Is.EqualTo(15));
            Assert.That(softwareVersion.Minor, Is.EqualTo(3));
            Assert.That(softwareVersion.Patch, Is.EqualTo(6));
        }

        [Test]
        public void Should_not_be_able_to_parse_invalid_version_string()
        {
            Assert.Throws<FormatException>(() => SoftwareVersion.Parse("12.ab.34"));
            Assert.Throws<OverflowException>(() => SoftwareVersion.Parse("12.9223372036854775808.34"));
            Assert.Throws<ArgumentException>(() => SoftwareVersion.Parse("12"));
            Assert.Throws<ArgumentNullException>(() => SoftwareVersion.Parse(null));
            Assert.Throws<ArgumentOutOfRangeException>(() => SoftwareVersion.Parse("1.-1.33"));
        }

        [Test]
        public void Should_be_able_to_get_version_string()
        {
            Assert.That(new SoftwareVersion(1,4,18).ToString(), Is.EqualTo("1.4.18"));
        }

        [Test]
        public void Should_be_able_to_clone_version()
        {
            var original = new SoftwareVersion(4, 8, 3);
            var clone = (SoftwareVersion)original.Clone();
            Assert.That(clone, Is.Not.SameAs(original));
            Assert.That(clone.Major, Is.EqualTo(original.Major));
            Assert.That(clone.Minor, Is.EqualTo(original.Minor));
            Assert.That(clone.Patch, Is.EqualTo(original.Patch));
        }

        [Test]
        public void Should_be_able_to_equate_two_identical_version()
        {
            var version1 = new SoftwareVersion(3, 4, 5);
            var version2 = new SoftwareVersion(3, 4, 5);
            Assert.That(version1, Is.EqualTo(version2));

            var version3 = new SoftwareVersion(3, 17, 5);
            Assert.That(version1, Is.Not.EqualTo(version3));
        }

        [Test]
        public void Should_be_able_to_equate_two_identical_version_as_objects()
        {
            var version1 = new SoftwareVersion(3, 4, 5);
            Object version2 = new SoftwareVersion(3, 4, 5);
            Assert.That(version1, Is.EqualTo(version2));

            Object version3 = new SoftwareVersion(3, 17, 5);
            Assert.That(version1, Is.Not.EqualTo(version3));
        }

        [Test]
        [TestCase("1.2.3", "1.2.3", 0)]
        [TestCase("1.3.3", "1.2.3", 1)]
        [TestCase("0.3.3", "1.2.3", -1)]
        public void Should_be_able_to_compare_versions(string versionA, string versionB, int result)
        {
            var version1 = SoftwareVersion.Parse(versionA);
            var version2 = SoftwareVersion.Parse(versionB);

            Assert.That(version1.CompareTo(version2), Is.EqualTo(result));
        }

        [Test]
        [TestCase("1.2.3", "1.2.3", 0)]
        [TestCase("1.3.3", "1.2.3", 1)]
        [TestCase("0.3.3", "1.2.3", -1)]
        public void Should_be_able_to_compare_versions_objects(string versionA, string versionB, int result)
        {
            var version1 = SoftwareVersion.Parse(versionA);
            var version2 = SoftwareVersion.Parse(versionB);

            Assert.That(version1.CompareTo((object) version2), Is.EqualTo(result));
        }

        [Test]
        public void Operator_overload_equals_should_identify_equal_versions()
        {
            var version1 = SoftwareVersion.Parse("1.2.3");
            var version2 = SoftwareVersion.Parse("1.2.3");
            Assert.That(version1 == version2, Is.True);

            var version3 = SoftwareVersion.Parse("1.2.4");
            Assert.That(version1 == version3, Is.False);
        }

        [Test]
        public void Operator_overload_not_equals_should_identify_unequal_versions()
        {
            var version1 = SoftwareVersion.Parse("1.2.3");
            var version2 = SoftwareVersion.Parse("1.2.4");
            Assert.That(version1 != version2, Is.True);

            var version3 = SoftwareVersion.Parse("1.2.3");
            Assert.That(version1 != version3, Is.False);
        }

        [Test]
        public void Operator_overload_less_than_should_identify_less_than()
        {
            var version1 = SoftwareVersion.Parse("1.2.3");
            var version2 = SoftwareVersion.Parse("1.2.4");
            Assert.That(version1 < version2, Is.True);

            var version3 = SoftwareVersion.Parse("1.2.3");
            Assert.That(version1 < version3, Is.False);
        }

        [Test]
        public void Operator_overload_greater_than_should_identify_greater_than()
        {
            var version1 = SoftwareVersion.Parse("1.2.3");
            var version2 = SoftwareVersion.Parse("1.2.2");
            Assert.That(version1 > version2, Is.True);

            var version3 = SoftwareVersion.Parse("1.2.3");
            Assert.That(version1 > version3, Is.False);
        }

        [Test]
        public void Operator_overload_less_than_or_equal_to_should_identify_less_than_or_equal_to()
        {
            var version1 = SoftwareVersion.Parse("1.2.3");
            var version2 = SoftwareVersion.Parse("1.2.3");
            Assert.That(version1 <= version2, Is.True);

            var version3 = SoftwareVersion.Parse("1.2.2");
            Assert.That(version1 <= version3, Is.False);
        }

        [Test]
        public void Operator_overload_greater_than_or_equal_to_should_identify_greater_than_or_equal_to()
        {
            var version1 = SoftwareVersion.Parse("1.2.3");
            var version2 = SoftwareVersion.Parse("1.2.3");
            Assert.That(version1 >= version2, Is.True);

            var version3 = SoftwareVersion.Parse("1.2.4");
            Assert.That(version1 >= version3, Is.False);
        }
    }
}