using System;
using FluentAssertions;
using NUnit.Framework;
using StockMarket.Trader.State;

// ReSharper disable InconsistentNaming
namespace StockMarket.Trader.Tests
{
    [TestFixture]
    public class StateProviderTests
    {
        public IStateProvider Given_new_state_provider()
        {
            return new StateProvider();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-10)]
        [TestCase(short.MinValue)]
        public void Should_throw_when_price_is_out_of_range(short price)
        {
            var stateProvider = Given_new_state_provider();

            Action action = () =>
                         {
                             stateProvider.CalculateState(price, 0);
                         };

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }


        [Test]
        [TestCase(0,0)]
        [TestCase(short.MaxValue,0)]
        [TestCase(0,3)]
        [TestCase(0,4)]
        [TestCase(0,5)]
        [TestCase(0,8)]
        [TestCase(0,11)]
        [TestCase(0,13)]
        [TestCase(0,15)]
        [TestCase(13343,0)]
        [TestCase(1234,15)]
        [TestCase(short.MaxValue,30)]
        [TestCase(short.MaxValue,short.MaxValue)]
        public void Should_calculate_state_and_reverse_to_price_and_sell_point(short price, short sellPoint)
        {
            var stateProvider = Given_new_state_provider();

            var state = stateProvider.CalculateState(price, sellPoint);

            Assert.That(stateProvider.GetPrice(state), Is.EqualTo(price));
            Assert.That(stateProvider.GetSellPoint(state), Is.EqualTo(sellPoint));
        }

    }
}