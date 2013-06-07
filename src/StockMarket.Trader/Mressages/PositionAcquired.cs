using System;

namespace StockMarket.Trader.Mressages
{
    public class PositionAcquired
    {
        public short Price { get; set; }

        public PositionAcquired()
        {
        }

        public PositionAcquired(short price)
        {
            Price = price;
        }
    }
}
