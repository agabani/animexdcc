using System;
using System.Threading.Tasks;
using AnimeXdcc.Core.SystemWrappers;
using NUnit.Framework;

namespace AnimeXdcc.Core.Tests.Unit.SystemWrappers
{
    [TestFixture]
    public class TimerWrapperTests
    {
        [Test]
        public void Should_invoke_event()
        {
            bool invoked;

            using (var timerWrapper = new TimerWrapper(1))
            {
                invoked = false;

                timerWrapper.Elapsed += (sender, args) => { invoked = true; };

                timerWrapper.Start();

                Task.Delay(20).Wait();
            }

            Assert.That(invoked, Is.True);
        }

        [Test]
        public void Should_provide_correct_time()
        {
            var expected = DateTime.MinValue;
            var actual = DateTime.MaxValue;

            using (var timerWrapper = new TimerWrapper(1))
            {
                timerWrapper.Elapsed += (sender, args) =>
                {
                    expected = DateTime.Now;
                    actual = args.SignalTime;
                };

                timerWrapper.Start();

                Task.Delay(20).Wait();
            }

            var timeSpan = expected - actual;

            Assert.That(timeSpan, Is.EqualTo(new TimeSpan(0)));
        }
    }
}