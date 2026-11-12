namespace TANet.Contracts.OperationResults.Indicators
{
    public class IchimokuResult : IndicatorResultBase
    {
        public double[] TenkanSen { get; set; }
        public double[] KijunSen { get; set; }
        public double[] SenkouSpanA { get; set; }
        public double[] SenkouSpanB { get; set; }
    }
}
