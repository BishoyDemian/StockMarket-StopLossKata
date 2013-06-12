using System;
using StockMarket.Trader.Mressages;

namespace StockMarket.Trader
{
    public interface IStockTrader
    {
        void AquirePosition(PositionAcquired newPosition);
        
        void UpdatePriceAsync(PriceChanged newPrice);

        short GetCurrentPrice();

        short GetCurrentSellPoint();

        event EventHandler SellNow;
    }
}
