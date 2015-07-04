using System;
using System.Timers;
using Generic.DccClient.Models;
using Generic.DccClient.Publishers;
using Generic.DccClient.SystemWrappers;
using Moq;
using NUnit.Framework;

namespace Generic.DccClient.Tests
{
    [TestFixture]
    public class TransferPublisherTests
    {
        [Test]
        public void Should_start_timer_when_created()
        {
            var timer = new Mock<ITimer>();
            var stopwatch = new Mock<IStopwatch>();

            var publisher = new TransferStatusPublisher2(null, 1, 1000, timer.Object, stopwatch.Object);

            timer.Verify(x => x.Start(), Times.Once);
        }

        [Test]
        public void Should_start_stopwatch_when_created()
        {
            var stopwatch = new Mock<IStopwatch>();
            var timer = new Mock<ITimer>();

            var publisher = new TransferStatusPublisher2(null, 1, 1000, timer.Object, stopwatch.Object);

            stopwatch.Verify(x => x.Start(), Times.Once);
        }

        [Test]
        public void Should_call_action_when_event_raised()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            var called = false;
            Action<TransferStatus> action = delegate { called = true; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(called, Is.True);
        }

        [Test]
        public void Should_return_object_when_event_raised()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus, Is.Not.Null);
        }

        [Test]
        public void Should_provide_object_with_correct_id()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus.TransferId, Is.EqualTo(1));
        }

        [Test]
        public void Should_provide_object_with_correct_total_bytes()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus.TotalBytes, Is.EqualTo(1000));
        }

        [Test]
        public void Should_provide_object_with_correct_transfered_bytes()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);
            publisher.Publish(100);
            publisher.Publish(50);
            publisher.Publish(130);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus.TransferedBytes, Is.EqualTo(280));
        }

        [Test]
        public void Should_provide_object_with_correct_elapsed_time()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            stopwatch
                .Setup(x => x.ElapsedMilliseconds)
                .Returns(3500);
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus.ElapsedTimeMilliseconds, Is.EqualTo(3500));
        }

        [Test]
        public void Should_provide_object_with_correct_average_speed_using_moving_average()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 1000, timer.Object, stopwatch.Object);

            publisher.Publish(10000);
            publisher.Publish(5000);
            publisher.Publish(25000);
            publisher.Publish(10000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            publisher.Publish(10000);
            publisher.Publish(5000);
            publisher.Publish(25000);
            publisher.Publish(20000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            publisher.Publish(10000);
            publisher.Publish(5000);
            publisher.Publish(25000);
            publisher.Publish(30000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);


            publisher.Publish(10000);
            publisher.Publish(5000);
            publisher.Publish(25000);
            publisher.Publish(40000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            publisher.Publish(10000);
            publisher.Publish(5000);
            publisher.Publish(25000);
            publisher.Publish(50000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            publisher.Publish(10000);
            publisher.Publish(5000);
            publisher.Publish(25000);
            publisher.Publish(60000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus.TransferSpeedBytesPerMillisecond, Is.EqualTo(80));
        }

        [Test]
        public void Should_provide_object_with_correct_remaining_time()
        {
            var timer = new Mock<ITimer>();
            timer
                .Setup(t => t.Interval)
                .Returns(1000);
            var stopwatch = new Mock<IStopwatch>();
            TransferStatus transferStatus = null;
            Action<TransferStatus> action = delegate(TransferStatus status) { transferStatus = status; };
            var publisher = new TransferStatusPublisher2(action, 1, 250000, timer.Object, stopwatch.Object);

            publisher.Publish(10000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            publisher.Publish(10000);

            timer.Raise(m => m.Elapsed += null, new EventArgs() as ElapsedEventArgs);

            Assert.That(transferStatus.RemainingTimeMilliseconds, Is.EqualTo(23000));
        }
    }
}