
namespace TANet.Contracts.OperationResults.Indicators
{
    public class StochasticResult : IndicatorResultBase
    {
        public decimal[] SlowK { get; set; }
        public decimal[] SlowD { get; set; }        
    }
}
