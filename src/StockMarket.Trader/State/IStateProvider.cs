using System;

namespace StockMarket.Trader.State
{
    public interface IStateProvider
    {
        int CalculateState(short price, byte secondsSincePrice);

        short GetPrice(int state);

        byte GetSecondsSinceLastPrice(int state);
    }
}
