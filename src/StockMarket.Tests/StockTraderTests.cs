using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StockMarket.Trader.Mressages;
using StockMarket.Trader.State;
using StockMarket.Trader.Time;

// ReSharper disable InconsistentNaming
namespace StockMarket.Trader.Tests
{
    [TestFixture]
    public class StockTraderTests
    {
        private IStateProvider _stateProvider;
        private ITimeProvider _timeProvider;


        public IStockTrader Given_new_trader()
        {
            _timeProvider = new TimeProvider();
            _stateProvider = new StateProvider();

            return new StockTrader(_timeProvider, _stateProvider);
        }

        [Test]
        public void Should_throw_for_null_message()
        {
            var stockTrader = Given_new_trader();

            Action action = () => { stockTrader.AquirePosition(null); };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(short.MinValue)]
        public void Should_throw_for_invalid_price(short price)
        {
            var stockTrader = Given_new_trader();

            Action action = () => { stockTrader.AquirePosition(new PositionAcquired(price)); };

            action.ShouldThrow<ArgumentException>();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(short.MaxValue)]
        public void Should_aquire_new_position(short price)
        {
            var stockTrader = Given_new_trader();

            stockTrader.AquirePosition(new PositionAcquired(price));

            Assert.That(stockTrader.GetCurrentPrice(), Is.EqualTo(price));
        }

        [Test]
        [TestCase(15)]
        [TestCase(30)]
        public void Should_keep_track_of_time(byte time)
        {
            var stockTrader = Given_new_trader();

            stockTrader.AquirePosition(new PositionAcquired(100));
            
            Task.Delay(TimeSpan.FromSeconds(time)).Wait();

            Assert.That(stockTrader.GetTimeSinceLastPriceUpdate(), Is.EqualTo(time));
        }
    }
}