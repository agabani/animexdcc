using System;
using System.Timers;
using AnimeXdcc.Core.Components.Publishers.Download;
using AnimeXdcc.Core.Dcc.Models;
using AnimeXdcc.Core.SystemWrappers;
using Moq;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Unit.Components.Publishers.Download
{
    [TestFixture]
    public class DccDownloadStatusPublisherTests
    {
        [Test]
        public void Should_invoke_start_timer()
        {
            var mockTimer = new Mock<ITimer>();

            new DccDownloadStatusPublisher(mockTimer.Object, 0, 0).Start();

            mockTimer
                .Verify(m => m.Start(), Times.Once());
        }

        [Test]
        public void Should_invoke_stop_timer()
        {
            var mockTimer = new Mock<ITimer>();

            new DccDownloadStatusPublisher(mockTimer.Object, 0, 0).Stop();

            mockTimer
                .Verify(m => m.Stop(), Times.Once());
        }

        [Test]
        public void Should_be_able_to_invoke_event()
        {
            var mockTimer = new Mock<ITimer>();

            var publisher = new DccDownloadStatusPublisher(mockTimer.Object, 0, 0);

            var raised = false;

            publisher.TransferStatus += (sender, status) => { raised = true; };

            mockTimer
                .Raise(e => e.Elapsed += null, new TimeElapsedEventArgs());

            Assert.That(raised, Is.True);
        }

        [Test]
        public void Should_return_valid_dcc_status_object()
        {
            var mockTimer = new Mock<ITimer>();

            mockTimer
                .Setup(p => p.Interval)
                .Returns(10);

            var publisher = new DccDownloadStatusPublisher(mockTimer.Object, 100, 0);

            DccTransferStatus s = null;

            publisher.TransferStatus += (sender, status) =>
            {
                s = status;
            };

            publisher.Update(50);

            mockTimer
                .Raise(e => e.Elapsed += null, new TimeElapsedEventArgs());

            mockTimer
                .Raise(e => e.Elapsed += null, new TimeElapsedEventArgs());

            Assert.That(s, Is.Not.Null);
            Assert.That(s.FileSize, Is.EqualTo(100));
            Assert.That(s.DownloadedBytes, Is.EqualTo(50));
            Assert.That(s.ElapsedTime, Is.EqualTo(20));
            Assert.That(s.BytesPerMillisecond, Is.EqualTo(2.5));
        }
    }
}