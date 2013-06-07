using System;
using System.Collections.Specialized;

namespace StockMarket.Trader.State
{
    public class StateProvider : IStateProvider
    {
        public const byte MaxTimeValue = byte.MaxValue;
        public const short MaxPriceValue = short.MaxValue;

        private readonly BitVector32.Section _timeSection;
        private readonly BitVector32.Section _priceSection;

        public StateProvider()
        {
            _timeSection = BitVector32.CreateSection(MaxTimeValue);
            _priceSection = BitVector32.CreateSection(MaxPriceValue, _timeSection);
        }

        public int CalculateState(short price, byte secondsSincePrice)
        {
            if (price < 0 || price > MaxPriceValue)
                throw new ArgumentOutOfRangeException("price");

            var bitVector = new BitVector32(0);
            bitVector[_timeSection] = secondsSincePrice;
            bitVector[_priceSection] = price;
            return bitVector.Data;
        }

        public short GetPrice(int state)
        {
            var bitVector = new BitVector32(state);
            return Convert.ToInt16(bitVector[_priceSection]);
        }

        public byte GetSecondsSinceLastPrice(int state)
        {
            var bitVector = new BitVector32(state);
            return Convert.ToByte(bitVector[_timeSection]);
        }
    }
}