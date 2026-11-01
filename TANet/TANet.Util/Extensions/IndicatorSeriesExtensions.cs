using System;
using System.Linq;


namespace TANet.Util.Extensions;


public static class IndicatorSeriesExtensions
{
    public static bool HasValidData(this double[] values)
        => values != null && values.Length > 0 && double.IsFinite(values[0]);

    public static int CountValidData(this double[] values)
        => values.Count(double.IsFinite);
    
}
