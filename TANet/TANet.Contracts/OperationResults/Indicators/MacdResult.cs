
namespace TANet.Contracts.OperationResults.Indicators
{
    public class MacdResult : IndicatorResultBase
    {
        public decimal[] Macd { get; set; }
        public decimal[] Signal { get; set; }
        public decimal[] Histogram { get; set; }
    }
}
