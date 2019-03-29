using System;
using System.Collections.Generic;
using System.Text;

namespace GOAT.Readers
{
    public abstract class PriceCandleProvider
    {
        public string CurrencyPair { get; }

        public PriceCandleProvider(String currencyPair)
        {
            CurrencyPair = currencyPair;
        }

        public abstract IEnumerable<Candle> GetCandles(DateTime since, DateTime to);

        public Tuple<DateTime, DateTime> GetAvailablePeriod()
        {
            throw new NotImplementedException();
        }
    }
}
