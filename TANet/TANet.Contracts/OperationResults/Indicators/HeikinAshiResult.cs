namespace TANet.Contracts.OperationResults.Indicators
{
    public class HeikinAshiResult : IndicatorResultBase
    {
        public double[] Open { get; set; }
        public double[] High { get; set; }
        public double[] Low { get; set; }
        public double[] Close { get; set; }
    }
}
