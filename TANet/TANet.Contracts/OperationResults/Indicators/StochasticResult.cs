
namespace TANet.Contracts.OperationResults.Indicators
{
    public class StochasticResult : IndicatorResultBase
    {
        public double[] SlowK { get; set; }
        public double[] SlowD { get; set; }        
    }
}
