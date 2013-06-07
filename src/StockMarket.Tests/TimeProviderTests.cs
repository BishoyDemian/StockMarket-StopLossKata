using System;
using System.Threading.Tasks;
using NUnit.Framework;
using StockMarket.Trader.Time;

// ReSharper disable InconsistentNaming
namespace StockMarket.Trader.Tests
{
    [TestFixture]
    public class TimeProviderTests
    {
        public TimeProvider Given_new_time_provider()
        {
            return new TimeProvider();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(30)]
        [TestCase(60)]
        public void Should_execute_callback_after_given_seconds(byte seconds)
        {
            var timeProvider = Given_new_time_provider();

            bool called = false;
            DateTime startTime = DateTime.Now;
            TimeSpan calledAfter = TimeSpan.MinValue;

            Action callback = () =>
                              {
                                  calledAfter = DateTime.Now - startTime;
                                  called = true;
                              };

            startTime = DateTime.Now;

            timeProvider.After(seconds, callback);

            Task.Delay(TimeSpan.FromSeconds(seconds + 1)).Wait();
            Assert.That(calledAfter.TotalSeconds, Is.EqualTo(seconds).Within(0.02));
            Assert.That(called, Is.True);
        }
    }
}