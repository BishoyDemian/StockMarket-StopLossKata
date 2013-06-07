using System;

namespace StockMarket.Trader.Time
{
    public interface ITimeProvider
    {
        void After(int seconds, Action<short> action, short param);
    }
}
