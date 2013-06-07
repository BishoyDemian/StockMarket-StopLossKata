using System;
using StockMarket.Trader.Mressages;

namespace StockMarket.Trader
{
    public interface IStockTrader
    {
        void AquirePosition(PositionAcquired newPosition);
        
        void UpdatePrice(PriceChanged newPrice);

        short GetCurrentPrice();

        event EventHandler SellNow;
    }
}
