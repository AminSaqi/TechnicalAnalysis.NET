using System;
using System.Collections.Generic;
using System.Text;

namespace TANet.Contracts.Models
{
    public class Candle
    {
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public int TradesCount { get; set; }
        public bool Finalized { get; set; }

        public DateTimeOffset OpenTime { get; set; }
        public DateTimeOffset CloseTime { get; set; }
    }
}
