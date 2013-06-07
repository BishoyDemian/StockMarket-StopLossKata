using System;

namespace StockMarket.Trader.State
{
    public interface IStateProvider
    {
        int CalculateState(short price, short sellPoint);

        short GetPrice(int state);

        short GetSellPoint(int state);
    }
}
