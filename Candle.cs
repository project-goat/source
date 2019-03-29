using System;
using System.Collections.Generic;
using System.Text;

namespace GOAT
{
    public class Candle
    {

        public decimal OpenPrice { get; }
        public decimal ClosePrice { get; }
        public decimal MaxPrice { get; }
        public decimal MinPrice { get; }

        public CandleTypeEnum Type { get; }

        public Candle(decimal open, decimal close, decimal max, decimal min)
        {
            if (open < close) Type = CandleTypeEnum.Increase;
            else Type = CandleTypeEnum.Decrease;

            OpenPrice = open;
            ClosePrice = close;
            MaxPrice = max;
            MinPrice = min;
        }
    }
}
