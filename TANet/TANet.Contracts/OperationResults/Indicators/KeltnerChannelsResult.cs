namespace TANet.Contracts.OperationResults.Indicators
{
    public class KeltnerChannelsResult : IndicatorResultBase
    {
        public double[] UpperBand { get; set; }
        public double[] MiddleBand { get; set; }
        public double[] LowerBand { get; set; }
    }
}
