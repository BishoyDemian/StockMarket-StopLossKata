using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using StockMarket.Trader.Mressages;
using StockMarket.Trader.State;

// ReSharper disable InconsistentNaming
namespace StockMarket.Trader.Tests
{
    [TestFixture]
    public class StockTraderTests
    {
        private IStateProvider _stateProvider;


        public StockTrader Given_new_trader()
        {
            _stateProvider = new StateProvider();

            return new StockTrader(_stateProvider);
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
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(40)]
        public void Should_not_update_sell_point_after_15_seconds_if_new_price_is_lower(short price)
        {
            var stockTrader = Given_new_trader();

            stockTrader.AquirePosition(new PositionAcquired(short.MaxValue));

            stockTrader.UpdatePriceAsync(new PriceChanged(price));

            Task.Delay(TimeSpan.FromSeconds(15)).Wait();
            Assert.That(stockTrader.GetCurrentSellPoint(), Is.EqualTo(short.MaxValue));
        }

        [Test]
        // test cases should be greater than 10 in order to trigger the update_sell_point
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(40)]
        public void Should_update_sell_point_after_15_seconds_if_new_price_is_higher(short price)
        {
            var stockTrader = Given_new_trader();

            stockTrader.AquirePosition(new PositionAcquired(10));

            stockTrader.UpdatePriceAsync(new PriceChanged(price));
            
            Task.Delay(TimeSpan.FromSeconds(14)).Wait();
            Assert.That(stockTrader.GetCurrentSellPoint(), Is.EqualTo(10));
            
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            Assert.That(stockTrader.GetCurrentSellPoint(), Is.EqualTo(Convert.ToInt16(Math.Floor(price * 0.9))));
        }

        [Test]
        [TestCase(20)]
        [TestCase(30)]
        [TestCase(40)]
        public void Should_stop_loss_after_30_seconds_if_new_price_is_lower(short price)
        {
            var stockTrader = Given_new_trader();
            var hasLossStopped = false;

            stockTrader.SellNow += (sender, args) =>
                                   {
                                       hasLossStopped = true;
                                   };

            stockTrader.AquirePosition(new PositionAcquired(short.MaxValue));

            stockTrader.UpdatePriceAsync(new PriceChanged(price));
            
            Task.Delay(TimeSpan.FromSeconds(29)).Wait();
            Assert.That(hasLossStopped, Is.False);
            
            Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            Assert.That(hasLossStopped, Is.True);
        }
    }
}