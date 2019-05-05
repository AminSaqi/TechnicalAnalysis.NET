using System;
using System.Collections.Generic;
using System.Text;

namespace TANet.Contracts.Models
{
    public class Candle
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public int TradesCount { get; set; }
        public bool Finalized { get; set; }

        public DateTimeOffset OpenTime { get; set; }
        public DateTimeOffset CloseTime { get; set; }
    }
}
