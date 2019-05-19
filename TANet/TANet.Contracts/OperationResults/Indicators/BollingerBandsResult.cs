
namespace TANet.Contracts.OperationResults.Indicators
{
    public class BollingerBandsResult : IndicatorResultBase
    {
        public decimal[] UpperBand { get; set; }
        public decimal[] MiddleBand { get; set; }
        public decimal[] LowerBand { get; set; }
    }
}
