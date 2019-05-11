using System;
using System.Collections.Generic;
using System.Linq;
using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Contracts.OperationResults.Indicators;

namespace TANet.Core
{
    public static class Indicators
    {
        #region EMA

        public static MovingAverageResult Ema(decimal[] input, int period)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Ema, period);
        }
        public static MovingAverageResult Ema(List<Candle> input, int period)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Ema, period);
        }
        public static MovingAverageResult Ema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Ema, period, calculationBase: calculationBase);
        }

        #endregion

        #region ExtendedMacd

        public static ExtendedMacdResult ExtendedMacd(decimal[] input, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            return Indicators.ExtendedMacd(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod);
        }
        public static ExtendedMacdResult ExtendedMacd(decimal[] input, int fastPeriod, int slowPeriod, int signalPeriod, MacdSignalType signalType)
        {
            return Indicators.ExtendedMacd(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, signalType: signalType);
        }
        public static ExtendedMacdResult ExtendedMacd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            return Indicators.ExtendedMacd(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod);
        }
        public static ExtendedMacdResult ExtendedMacd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod, MacdSignalType signalType)
        {
            return Indicators.ExtendedMacd(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, signalType: signalType);
        }
        public static ExtendedMacdResult ExtendedMacd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod, IndicatorCalculationBase calculationBase)
        {
            return Indicators.ExtendedMacd(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, calculationBase: calculationBase);
        }
        public static ExtendedMacdResult ExtendedMacd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod, MacdSignalType signalType, IndicatorCalculationBase calculationBase)
        {
            return Indicators.ExtendedMacd(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, signalType: signalType, calculationBase: calculationBase);
        }

        #endregion

        #region RSI

        public static RsiResult Rsi(decimal[] input, int period)
        {
            return RelativeStrengthIndex(input, period, null);
        }
        public static RsiResult Rsi(decimal[] input, int period, Func<decimal[], IndicatorSignal> signalLogic)
        {
            return RelativeStrengthIndex(input, period, signalLogic);
        }
        public static RsiResult Rsi(List<Candle> input, int period)
        {
            return RelativeStrengthIndex(input, period);
        }
        public static RsiResult Rsi(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return RelativeStrengthIndex(input, period, calculationBase: calculationBase);
        }

        #endregion

        #region SMA

        public static MovingAverageResult Sma(decimal[] input, int period)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Sma, period);
        }
        public static MovingAverageResult Sma(List<Candle> input, int period)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Sma, period);
        }
        public static MovingAverageResult Sma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MovingAverage(input, MovingAverageType.Sma, period, calculationBase: calculationBase);
        }

        #endregion

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

        #region Private Methods

        private static ExtendedMacdResult ExtendedMacd(List<Candle> candles, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod, IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close, MacdSignalType signalType = MacdSignalType.ZeroLineCrossover)
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

            return ExtendedMacd(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod);
        }
        private static ExtendedMacdResult ExtendedMacd(decimal[] input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod, MacdSignalType signalType = MacdSignalType.ZeroLineCrossover)
        {
            return TANet.Util.StaticClasses.Indicators.MacdExt(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, signalType);
        }

        private static MovingAverageResult MovingAverage(List<Candle> candles, MovingAverageType maType, int period, IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close)
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
        private static MovingAverageResult MovingAverage(decimal[] input, MovingAverageType maType, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Ma(input, maType, period);
        }

        private static RsiResult RelativeStrengthIndex(List<Candle> candles, 
            int period, 
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close, 
            Func<decimal[], IndicatorSignal> signalLogic = null)
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

            return Rsi(input, period, signalLogic);
        }
        private static RsiResult RelativeStrengthIndex(decimal[] input,
            int period,
            Func<decimal[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Rsi(input, period, signalLogic);
        }

        #endregion
    }
}
