using System;
using System.Threading.Tasks;
using StockMarket.Trader.Mressages;
using StockMarket.Trader.State;

namespace StockMarket.Trader
{
    /*
     * "sell point" should only move up if it’s held for more than 15 seconds 
     * "stop loss" should only be triggered if the price point is held below the sell point for more than 30 seconds.
     */
    public class StockTrader : IStockTrader
    {
        private readonly IStateProvider _stateProvider;

        private int _state;

        public event EventHandler SellNow;

        public StockTrader(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        public void AquirePosition(PositionAcquired newPosition)
        {
            if (newPosition == null)
                throw new ArgumentNullException("newPosition");
            
            if (newPosition.Price <= 0)
                throw new ArgumentException("Price must be > 0 (zero)", "newPosition");

            _state = _stateProvider.CalculateState(newPosition.Price, newPosition.Price);
        }

        public async void UpdatePriceAsync(PriceChanged newPrice)
        {
            if (newPrice == null)
                throw new ArgumentNullException("newPrice");

            if (newPrice.Price <= 0)
                throw new ArgumentException("Price must be > 0 (zero)", "newPrice");

            var currentPrice = GetCurrentPrice();

            if (newPrice.Price == currentPrice)
                return;

            _state = _stateProvider.CalculateState(newPrice.Price, GetCurrentSellPoint());

            if (newPrice.Price > currentPrice)
            {
                await UpdateSellPoint(newPrice.Price);
            }
            else if (newPrice.Price < currentPrice)
            {
                await StopLoss(newPrice.Price);
            }
        }

        public short GetCurrentPrice()
        {
            return _stateProvider.GetPrice(_state);
        }

        public short GetCurrentSellPoint()
        {
            return _stateProvider.GetSellPoint(_state);
        }

        private async Task UpdateSellPoint(short newPrice)
        {
            await Task.Delay(TimeSpan.FromSeconds(15));

            var currentPrice = GetCurrentPrice();

            // if price dropped, abort operation
            if (currentPrice < newPrice)
                return;

            var sellPoint = Convert.ToInt16(Math.Floor(newPrice * 0.9));

            _state = _stateProvider.CalculateState(currentPrice, sellPoint);
        }

        private async Task StopLoss(short newPrice)
        {
            await Task.Delay(TimeSpan.FromSeconds(30));

            var currentPrice = GetCurrentPrice();

            // if price has changed while waiting...
            // just abort current stop loss and another one will be invoked anyway
            if (currentPrice > newPrice)
                return;

            var sellPoint = Convert.ToInt16(Math.Floor(newPrice * 0.9));
            _state = _stateProvider.CalculateState(currentPrice, sellPoint);

            if (SellNow != null)
                SellNow(this, EventArgs.Empty);

        }
    }
}