using System;
using System.Collections.Specialized;

namespace StockMarket.Trader.State
{
    public class StateProvider : IStateProvider
    {
        private readonly BitVector32.Section _priceSection;
        private readonly BitVector32.Section _sellPointSection;

        public StateProvider()
        {
            _sellPointSection = BitVector32.CreateSection(short.MaxValue);
            _priceSection = BitVector32.CreateSection(short.MaxValue, _sellPointSection);
        }

        public int CalculateState(short price, short sellPoint)
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException("price");

            var bitVector = new BitVector32(0);
            bitVector[_priceSection] = price;
            bitVector[_sellPointSection] = sellPoint;
            return bitVector.Data;
        }

        public short GetPrice(int state)
        {
            var bitVector = new BitVector32(state);
            return Convert.ToInt16(bitVector[_priceSection]);
        }

        public short GetSellPoint(int state)
        {
            var bitVector = new BitVector32(state);
            return Convert.ToInt16(bitVector[_sellPointSection]);
        }
    }
}