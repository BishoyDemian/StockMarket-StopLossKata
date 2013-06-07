using System;

namespace StockMarket.Trader.Mressages
{
    public class PriceChanged
    {
        public short Price { get; set; }

        public PriceChanged()
        {
        }

        public PriceChanged(short price)
        {
            Price = price;
        }
    }
}
