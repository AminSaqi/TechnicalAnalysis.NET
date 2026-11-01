
namespace TANet.Contracts.OperationResults.Indicators
{
    public class MacdResult : IndicatorResultBase
    {
        public double[] Macd { get; set; }
        public double[] Signal { get; set; }
        public double[] Histogram { get; set; }
    }
}
