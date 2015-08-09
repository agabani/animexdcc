using System;
using System.IO;
using AnimeXdcc.Core.Components.Files.Models;
using AnimeXdcc.Core.Components.Files.Services;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Integration.Components.Files
{
    [TestFixture]
    public class FileServiceTests
    {
        private FileService _fileService;
        private string _path;

        [SetUp]
        public void SetUp()
        {
            _path = Guid.NewGuid().ToString().Replace("-", string.Empty);
            _fileService = new FileService();
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_path))
            {
                File.Delete(_path);
            }
        }

        [Test]
        public void File_does_not_exist()
        {
            var fileDetails = _fileService.GetDetails(_path);

            Assert.That(fileDetails.FullPath, Is.StringContaining(_path));
            Assert.That(fileDetails.IsReadOnly, Is.EqualTo(false));
            Assert.That(fileDetails.Length, Is.EqualTo(0));
        }

        [Test]
        public void File_does_exist_empty()
        {
            CreateNewEmptyFile();

            var fileDetails = _fileService.GetDetails(_path);

            Assert.That(fileDetails.FullPath, Is.StringContaining(_path));
            Assert.That(fileDetails.IsReadOnly, Is.EqualTo(false));
            Assert.That(fileDetails.Length, Is.EqualTo(0));
        }

        [Test]
        public void File_does_exist_not_empty()
        {
            CreateNewNotEmptyFile();

            var fileDetails = _fileService.GetDetails(_path);

            Assert.That(fileDetails.FullPath, Is.StringContaining(_path));
            Assert.That(fileDetails.IsReadOnly, Is.EqualTo(false));
            Assert.That(fileDetails.Length, Is.EqualTo(36));
        }

        [Test]
        public void File_stream_no_file_overwrite_mode()
        {
            var fileDetails = _fileService.GetDetails(_path);

            using (var fileStream = _fileService.GetFileStream(fileDetails.FullPath, FileStrategy.OverWrite))
            {
                Assert.That(fileStream.Length, Is.EqualTo(0));
                Assert.That(fileStream.Position, Is.EqualTo(0));
                Assert.That(fileStream.CanWrite, Is.EqualTo(true));
            }
        }

        [Test]
        public void File_stream_empty_file_overwrite_mode()
        {
            CreateNewEmptyFile();

            var fileDetails = _fileService.GetDetails(_path);

            using (var fileStream = _fileService.GetFileStream(fileDetails.FullPath, FileStrategy.OverWrite))
            {
                Assert.That(fileStream.Length, Is.EqualTo(0));
                Assert.That(fileStream.Position, Is.EqualTo(0));
                Assert.That(fileStream.CanWrite, Is.EqualTo(true));
            }
        }

        [Test]
        public void File_stream_not_empty_overwrite_mode()
        {
            CreateNewNotEmptyFile();

            var fileDetails = _fileService.GetDetails(_path);

            using (var fileStream = _fileService.GetFileStream(fileDetails.FullPath, FileStrategy.OverWrite))
            {
                Assert.That(fileStream.Length, Is.EqualTo(0));
                Assert.That(fileStream.Position, Is.EqualTo(0));
                Assert.That(fileStream.CanWrite, Is.EqualTo(true));
            }
        }

        [Test]
        public void File_stream_no_file_resume_mode()
        {
            CreateNewEmptyFile();

            var fileDetails = _fileService.GetDetails(_path);

            using (var fileStream = _fileService.GetFileStream(fileDetails.FullPath, FileStrategy.Resume))
            {
                Assert.That(fileStream.Length, Is.EqualTo(0));
                Assert.That(fileStream.Position, Is.EqualTo(0));
                Assert.That(fileStream.CanWrite, Is.EqualTo(true));
            }
        }

        [Test]
        public void File_stream_empty_file_resume_mode()
        {
            CreateNewEmptyFile();

            var fileDetails = _fileService.GetDetails(_path);

            using (var fileStream = _fileService.GetFileStream(fileDetails.FullPath, FileStrategy.Resume))
            {
                Assert.That(fileStream.Length, Is.EqualTo(0));
                Assert.That(fileStream.Position, Is.EqualTo(0));
                Assert.That(fileStream.CanWrite, Is.EqualTo(true));
            }
        }

        [Test]
        public void File_stream_not_empty_resume_mode()
        {
            CreateNewNotEmptyFile();

            using (var fileStream = new StreamWriter(_path))
            {
                fileStream.Write(Guid.NewGuid().ToString());
            }

            var fileDetails = _fileService.GetDetails(_path);

            using (var fileStream = _fileService.GetFileStream(fileDetails.FullPath, FileStrategy.Resume))
            {
                Assert.That(fileStream.Length, Is.EqualTo(36));
                Assert.That(fileStream.Position, Is.EqualTo(36));
                Assert.That(fileStream.CanWrite, Is.EqualTo(true));
            }
        }

        private void CreateNewEmptyFile()
        {
            File.Create(_path).Dispose();
        }

        private void CreateNewNotEmptyFile()
        {
            using (var fileStream = new StreamWriter(_path))
            {
                fileStream.Write(Guid.NewGuid().ToString());
            }
        }
    }
}