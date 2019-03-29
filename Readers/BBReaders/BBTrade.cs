using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace GOAT
{
    [DataContract]
    public class BBTrade
    {
        [DataMember(Name = "date")]
        public long UnixDate { get; set; }

        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "type")]
#pragma warning disable IDE0044
        public String TypeBuySellString;
#pragma warning restore IDE0044

        [DataMember(Name = "amount")]
        public decimal Amount { get; set; }

        [DataMember(Name = "tid")]
        public long TID { get; set; }
        public DateTimeOffset Date { get => DateTimeOffset.FromUnixTimeSeconds(UnixDate); }

        public DateTimeOffset DateLocal { get => Date.ToLocalTime(); }

        public SellBuyEnum BuySell { get => TypeBuySellString[0] == 's' ? SellBuyEnum.Sell_Ask : SellBuyEnum.Buy_Bid; }

        
        public enum SellBuyEnum
        {
            Sell_Ask,
            Buy_Bid
        }
    }
}
