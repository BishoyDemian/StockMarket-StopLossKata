using System;
using StockMarket.Trader.Mressages;
using StockMarket.Trader.State;
using StockMarket.Trader.Time;

namespace StockMarket.Trader
{
    /*
     * "sell point" should only move up if it’s held for more than 15 seconds 
     * "stop loss" should only be triggered if the price point is held below the sell point for more than 30 seconds.
     */
    public class StockTrader : IStockTrader
    {
        private ITimeProvider _timeProvider;
        private IStateProvider _stateProvider;

        private int _state;

        public event EventHandler SellNow;

        public StockTrader(ITimeProvider timeProvider, IStateProvider stateProvider)
        {
            _timeProvider = timeProvider;
            _stateProvider = stateProvider;
        }

        public void AquirePosition(PositionAcquired newPosition)
        {
            if (newPosition == null)
                throw new ArgumentNullException("newPosition");
            
            if (newPosition.Price < 0)
                throw new ArgumentException("Price must be >= 0 (zero)", "newPosition");

            _state = _stateProvider.CalculateState(newPosition.Price, 0);
        }

        public void UpdatePrice(PriceChanged newPrice)
        {
            if (newPrice == null)
                throw new ArgumentNullException("newPrice");

            if (newPrice.Price < 0)
                throw new ArgumentException("Price must be >= 0 (zero)", "newPrice");

            _state = _stateProvider.CalculateState(newPrice.Price, 0);

            var currentPrice = GetCurrentPrice();

            if (newPrice.Price > currentPrice)
                _timeProvider.After(15, UpdateSellPoint, newPrice.Price);
            else if (newPrice.Price < currentPrice)
                _timeProvider.After(30, StopLoss, newPrice.Price);
        }

        public short GetCurrentPrice()
        {
            return _stateProvider.GetPrice(_state);
        }

        private void UpdateSellPoint(short priceWhenAskingForUpdate)
        {
            if (GetCurrentPrice() != priceWhenAskingForUpdate)
                return;
        }

        private void StopLoss(short priceWhenAskingForUpdate)
        {
            if (GetCurrentPrice() != priceWhenAskingForUpdate)
                return;

            if (SellNow != null)
                SellNow(this, EventArgs.Empty);
        }
    }
}