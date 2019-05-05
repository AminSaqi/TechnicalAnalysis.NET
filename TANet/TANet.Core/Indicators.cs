using System.Collections.Generic;
using System.Linq;
using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Contracts.OperationResults.Indicators;

namespace TANet.Core
{
    public static class Indicators
    {
        #region WMA

        public static MovingAverageResult Wma(decimal[] input, int period)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Wma, period);
        }
        public static MovingAverageResult Wma(List<Candle> input, int period)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Wma, period);
        }
        public static MovingAverageResult Wma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Wma, period, calculationBase: calculationBase);
        }

        #endregion

        public static MovingAverageResult MovingAverage(List<Candle> candles, MovingAverageType maType, int period, IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close)
        {
            decimal[] input;
            if (calculationBase == IndicatorCalculationBase.Close)
                input = candles.Select(c => c.Close).ToArray();

            else if (calculationBase == IndicatorCalculationBase.Open)
                input = candles.Select(c => c.Open).ToArray();

            else if (calculationBase == IndicatorCalculationBase.High)
                input = candles.Select(c => c.High).ToArray();

            else if (calculationBase == IndicatorCalculationBase.Low)
                input = candles.Select(c => c.Low).ToArray();

            else if (calculationBase == IndicatorCalculationBase.Volume)
                input = candles.Select(c => c.Volume).ToArray();

            else
                input = candles.Select(c => c.Close).ToArray();

            return MovingAverage(input, maType, period);
        }
        public static MovingAverageResult MovingAverage(decimal[] input, MovingAverageType maType, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Ma(input, maType, period);
        }
    }
}
