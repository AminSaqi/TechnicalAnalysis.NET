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
        #region ALMA

        public static MovingAverageResult Alma(double[] input, int period, double sigma, double offset)
        {
            return TANet.Util.StaticClasses.Indicators.Alma(input, period, sigma, offset);
        }
        public static MovingAverageResult Alma(double[] input, int period, double sigma, double offset, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Alma(input, period, sigma, offset, signalLogic);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset)
        {
            return Alma(input, period, sigma, offset, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Alma(input, period, sigma, offset, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset, IndicatorCalculationBase calculationBase)
        {
            return Alma(input, period, sigma, offset, calculationBase, null);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Alma(values, period, sigma, offset, signalLogic);
        }

        #endregion

        #region Aroon

        public static AroonResult Aroon(double[] high, double[] low, int period)
        {
            return AROON(high, low, period, null);
        }
        public static AroonResult Aroon(double[] high, double[] low, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return AROON(high, low, period, signalLogic);
        }
        public static AroonResult Aroon(List<Candle> candles, int period)
        {
            return AROON(candles, period);
        }
        public static AroonResult Aroon(List<Candle> candles, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return AROON(candles, period, signalLogic);
        }

        #endregion

        #region AroonOscillator

        public static AroonOscillatorResult AroonOscillator(double[] high, double[] low, int period)
        {
            return AROON_OSC(high, low, period, null);
        }
        public static AroonOscillatorResult AroonOscillator(double[] high, double[] low, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return AROON_OSC(high, low, period, signalLogic);
        }
        public static AroonOscillatorResult AroonOscillator(List<Candle> candles, int period)
        {
            return AROON_OSC(candles, period);
        }
        public static AroonOscillatorResult AroonOscillator(List<Candle> candles, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return AROON_OSC(candles, period, signalLogic);
        }

        #endregion

        #region ATR

        public static AtrResult Atr(double[] high, double[] low, double[] close, int period)
        {
            return ATR(high, low, close, period, null);
        }
        public static AtrResult Atr(double[] high, double[] low, double[] close, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return ATR(high, low, close, period, signalLogic);
        }
        public static AtrResult Atr(List<Candle> candles, int period)
        {
            return ATR(candles, period);
        }
        public static AtrResult Atr(List<Candle> candles, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return ATR(candles, period, signalLogic);
        }

        #endregion

        #region BollingerBands

        public static BollingerBandsResult BollingerBands(double[] input, int period, double standardDeviationUp, double standardDeviationDown)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, MovingAverageType.Sma);
        }
        public static BollingerBandsResult BollingerBands(double[] input, int period, double standardDeviationUp, double standardDeviationDown, Func<double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, MovingAverageType.Sma, signalLogic: signalLogic);
        }
        public static BollingerBandsResult BollingerBands(double[] input, int period, double standardDeviationUp, double standardDeviationDown, MovingAverageType maType)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, maType);
        }
        public static BollingerBandsResult BollingerBands(double[] input, int period, double standardDeviationUp, double standardDeviationDown, MovingAverageType maType, Func<double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, maType, signalLogic: signalLogic);
        }
        public static BollingerBandsResult BollingerBands(List<Candle> input, int period, double standardDeviationUp, double standardDeviationDown)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, MovingAverageType.Sma, IndicatorCalculationBase.Close);
        }
        public static BollingerBandsResult BollingerBands(List<Candle> input, int period, double standardDeviationUp, double standardDeviationDown, Func<double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, MovingAverageType.Sma, IndicatorCalculationBase.Close, signalLogic);
        }
        public static BollingerBandsResult BollingerBands(List<Candle> input, int period, double standardDeviationUp, double standardDeviationDown, IndicatorCalculationBase calculationBase)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, MovingAverageType.Sma, calculationBase: calculationBase);
        }
        public static BollingerBandsResult BollingerBands(List<Candle> input, int period, double standardDeviationUp, double standardDeviationDown, IndicatorCalculationBase calculationBase, Func<double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, MovingAverageType.Sma, calculationBase: calculationBase, signalLogic: signalLogic);
        }
        public static BollingerBandsResult BollingerBands(List<Candle> input, int period, double standardDeviationUp, double standardDeviationDown, MovingAverageType maType, IndicatorCalculationBase calculationBase)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, maType, calculationBase: calculationBase);
        }
        public static BollingerBandsResult BollingerBands(List<Candle> input, int period, double standardDeviationUp, double standardDeviationDown, MovingAverageType maType, IndicatorCalculationBase calculationBase, Func<double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return B_BANDS(input, period, standardDeviationUp, standardDeviationDown, maType, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region CCI

        public static CciResult Cci(double[] high, double[] low, double[] close, int period)
        {
            return CCI(high, low, close, period, null);
        }
        public static CciResult Cci(double[] high, double[] low, double[] close, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return CCI(high, low, close, period, signalLogic);
        }
        public static CciResult Cci(List<Candle> candles, int period)
        {
            return CCI(candles, period);
        }
        public static CciResult Cci(List<Candle> candles, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return CCI(candles, period, signalLogic);
        }

        #endregion

        #region DEMA

        public static MovingAverageResult Dema(double[] input, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Dema(input, period);
        }
        public static MovingAverageResult Dema(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Dema(input, period, signalLogic);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period)
        {
            return Dema(input, period, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Dema(input, period, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Dema(input, period, calculationBase, null);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Dema(values, period, signalLogic);
        }

        #endregion

        #region EMA

        public static MovingAverageResult Ema(double[] input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Ema, period);
        }
        public static MovingAverageResult Ema(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Ema, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Ema(List<Candle> input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Ema, period);
        }
        public static MovingAverageResult Ema(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Ema, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Ema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MA(input, MovingAverageType.Ema, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Ema(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Ema, period, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region ExtendedMacd

        public static MacdResult ExtendedMacd(double[] input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod)
        {
            return Indicators.MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod);
        }
        public static MacdResult ExtendedMacd(double[] input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, signalLogic: signalLogic);
        }
        public static MacdResult ExtendedMacd(List<Candle> input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod)
        {
            return Indicators.MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod);
        }
        public static MacdResult ExtendedMacd(List<Candle> input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, signalLogic: signalLogic);
        }
        public static MacdResult ExtendedMacd(List<Candle> input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, calculationBase: calculationBase);
        }
        public static MacdResult ExtendedMacd(List<Candle> input, MovingAverageType fastMaType, int fastPeriod, MovingAverageType slowMaType, int slowPeriod, MovingAverageType signalMaType, int signalPeriod, IndicatorCalculationBase calculationBase, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion        

        #region HMA

        public static MovingAverageResult Hma(double[] input, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Hma(input, period);
        }
        public static MovingAverageResult Hma(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Hma(input, period, signalLogic);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period)
        {
            return Hma(input, period, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Hma(input, period, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Hma(input, period, calculationBase, null);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Hma(values, period, signalLogic);
        }

        #endregion

        #region KAMA

        public static MovingAverageResult Kama(double[] input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Kama, period);
        }
        public static MovingAverageResult Kama(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Kama, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Kama(List<Candle> input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Kama, period);
        }
        public static MovingAverageResult Kama(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Kama, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Kama(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MA(input, MovingAverageType.Kama, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Kama(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Kama, period, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region MAMA

        public static MamaResult Mama(double[] input, double fastLimit, double slowLimit)
        {
            return TANet.Util.StaticClasses.Indicators.Mama(input, fastLimit, slowLimit);
        }
        public static MamaResult Mama(double[] input, double fastLimit, double slowLimit, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Mama(input, fastLimit, slowLimit, signalLogic);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit)
        {
            return Mama(input, fastLimit, slowLimit, IndicatorCalculationBase.Close, null);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Mama(input, fastLimit, slowLimit, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit, IndicatorCalculationBase calculationBase)
        {
            return Mama(input, fastLimit, slowLimit, calculationBase, null);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Mama(values, fastLimit, slowLimit, signalLogic);
        }

        #endregion

        #region MACD

        public static MacdResult Macd(double[] input, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            return Indicators.MACD(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod);
        }
        public static MacdResult Macd(double[] input, int fastPeriod, int slowPeriod, int signalPeriod, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MACD(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, signalLogic: signalLogic);
        }
        public static MacdResult Macd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            return Indicators.MACD(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod);
        }
        public static MacdResult Macd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MACD(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, signalLogic: signalLogic);
        }
        public static MacdResult Macd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MACD(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, calculationBase: calculationBase);
        }
        public static MacdResult Macd(List<Candle> input, int fastPeriod, int slowPeriod, int signalPeriod, IndicatorCalculationBase calculationBase, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MACD(input, MovingAverageType.Ema, fastPeriod, MovingAverageType.Ema, slowPeriod, MovingAverageType.Ema, signalPeriod, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region MFI

        public static MfiResult Mfi(double[] high, double[] low, double[] close, double[] volume, int period)
        {
            return MFI(high, low, close, volume, period, null);
        }
        public static MfiResult Mfi(double[] high, double[] low, double[] close, double[] volume, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return MFI(high, low, close, volume, period, signalLogic);
        }
        public static MfiResult Mfi(List<Candle> candles, int period)
        {
            return MFI(candles, period);
        }
        public static MfiResult Mfi(List<Candle> candles, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return MFI(candles, period, signalLogic);
        }

        #endregion

        #region ParabolicSar

        public static ParabolicSarResult ParabolicSar(double[] high, double[] low, double acceleration, double maximum)
        {
            return P_SAR(high, low, acceleration, maximum, null);
        }
        public static ParabolicSarResult ParabolicSar(double[] high, double[] low, double acceleration, double maximum, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return P_SAR(high, low, acceleration, maximum, signalLogic);
        }
        public static ParabolicSarResult ParabolicSar(List<Candle> candles, double acceleration, double maximum)
        {
            return P_SAR(candles, acceleration, maximum);
        }
        public static ParabolicSarResult ParabolicSar(List<Candle> candles, double acceleration, double maximum, Func<double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return P_SAR(candles, acceleration, maximum, signalLogic);
        }

        #endregion

        #region RMA

        public static MovingAverageResult Rma(double[] input, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Rma(input, period);
        }
        public static MovingAverageResult Rma(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Rma(input, period, signalLogic);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period)
        {
            return Rma(input, period, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Rma(input, period, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Rma(input, period, calculationBase, null);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Rma(values, period, signalLogic);
        }

        #endregion

        #region RSI

        public static RsiResult Rsi(double[] input, int period)
        {
            return RSI(input, period, null);
        }
        public static RsiResult Rsi(double[] input, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return RSI(input, period, signalLogic);
        }
        public static RsiResult Rsi(List<Candle> input, int period)
        {
            return RSI(input, period);
        }
        public static RsiResult Rsi(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return RSI(input, period, calculationBase: calculationBase);
        }

        #endregion

        #region SMA

        public static MovingAverageResult Sma(double[] input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Sma, period);
        }
        public static MovingAverageResult Sma(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Sma, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Sma(List<Candle> input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Sma, period);
        }
        public static MovingAverageResult Sma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Sma, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Sma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MA(input, MovingAverageType.Sma, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Sma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Sma, period, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region Stochastic

        public static StochasticResult Stochastic(List<Candle> candles, int fastKPeriod, int slowKPeriod, int slowDPeriod)
        {
            return STOCH(candles, fastKPeriod, MovingAverageType.Sma, slowKPeriod, MovingAverageType.Sma, slowDPeriod);
        }
        public static StochasticResult Stochastic(List<Candle> candles, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod)
        {
            return STOCH(candles, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod);
        }
        public static StochasticResult Stochastic(List<Candle> candles, int fastKPeriod, int slowKPeriod, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return STOCH(candles, fastKPeriod, MovingAverageType.Sma, slowKPeriod, MovingAverageType.Sma, slowDPeriod, signalLogic);
        }
        public static StochasticResult Stochastic(List<Candle> candles, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return STOCH(candles, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod, signalLogic);
        }
        public static StochasticResult Stochastic(double[] high, double[] low, double[] close, int fastKPeriod, int slowKPeriod, int slowDPeriod)
        {
            return STOCH(high, low, close, fastKPeriod, MovingAverageType.Sma, slowKPeriod, MovingAverageType.Sma, slowDPeriod);
        }
        public static StochasticResult Stochastic(double[] high, double[] low, double[] close, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod)
        {
            return STOCH(high, low, close, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod);
        }
        public static StochasticResult Stochastic(double[] high, double[] low, double[] close, int fastKPeriod, int slowKPeriod, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return STOCH(high, low, close, fastKPeriod, MovingAverageType.Sma, slowKPeriod, MovingAverageType.Sma, slowDPeriod, signalLogic);
        }
        public static StochasticResult Stochastic(double[] high, double[] low, double[] close, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return STOCH(high, low, close, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod, signalLogic);
        }

        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(List<Candle> candles, int fastKPeriod, int slowKPeriod, int slowDPeriod)
        {
            return Stochastic(candles, fastKPeriod, slowKPeriod, slowDPeriod);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(List<Candle> candles, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod)
        {
            return Stochastic(candles, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(List<Candle> candles, int fastKPeriod, int slowKPeriod, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Stochastic(candles, fastKPeriod, slowKPeriod, slowDPeriod, signalLogic);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(List<Candle> candles, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Stochastic(candles, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod, signalLogic);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(double[] high, double[] low, double[] close, int fastKPeriod, int slowKPeriod, int slowDPeriod)
        {
            return Stochastic(high, low, close, fastKPeriod, slowKPeriod, slowDPeriod);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(double[] high, double[] low, double[] close, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod)
        {
            return Stochastic(high, low, close, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(double[] high, double[] low, double[] close, int fastKPeriod, int slowKPeriod, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Stochastic(high, low, close, fastKPeriod, slowKPeriod, slowDPeriod, signalLogic);
        }
        [Obsolete("Use Stochastic instead.")]
        public static StochasticResult Stochsatic(double[] high, double[] low, double[] close, int fastKPeriod, MovingAverageType slowKMaType, int slowKPeriod, MovingAverageType slowDMaType, int slowDPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Stochastic(high, low, close, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod, signalLogic);
        }

        #endregion

        #region T3

        public static MovingAverageResult T3(double[] input, int period, double vFactor)
        {
            return TANet.Util.StaticClasses.Indicators.T3(input, period, vFactor);
        }
        public static MovingAverageResult T3(double[] input, int period, double vFactor, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.T3(input, period, vFactor, signalLogic);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor)
        {
            return T3(input, period, vFactor, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return T3(input, period, vFactor, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor, IndicatorCalculationBase calculationBase)
        {
            return T3(input, period, vFactor, calculationBase, null);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.T3(values, period, vFactor, signalLogic);
        }

        #endregion

        #region TEMA

        public static MovingAverageResult Tema(double[] input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Tema, period);
        }
        public static MovingAverageResult Tema(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Tema, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Tema(List<Candle> input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Tema, period);
        }
        public static MovingAverageResult Tema(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Tema, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Tema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MA(input, MovingAverageType.Tema, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Tema(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Tema, period, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region TRIMA

        public static MovingAverageResult Trima(double[] input, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Trima(input, period);
        }
        public static MovingAverageResult Trima(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Trima(input, period, signalLogic);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period)
        {
            return Trima(input, period, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Trima(input, period, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Trima(input, period, calculationBase, null);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Trima(values, period, signalLogic);
        }

        #endregion

        #region WilliamsR

        public static WilliamsRResult WilliamsR(double[] high, double[] low, double[] close, double[] volume, int period)
        {
            return WILL_R(high, low, close, period, null);
        }
        public static WilliamsRResult WilliamsR(double[] high, double[] low, double[] close, double[] volume, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return WILL_R(high, low, close, period, signalLogic);
        }
        public static WilliamsRResult WilliamsR(List<Candle> candles, int period)
        {
            return WILL_R(candles, period);
        }
        public static WilliamsRResult WilliamsR(List<Candle> candles, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return WILL_R(candles, period, signalLogic);
        }

        #endregion

        #region VWMA

        public static MovingAverageResult Vwma(double[] input, double[] volume, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Vwma(input, volume, period);
        }
        public static MovingAverageResult Vwma(double[] input, double[] volume, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Vwma(input, volume, period, signalLogic);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period)
        {
            return Vwma(input, period, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Vwma(input, period, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Vwma(input, period, calculationBase, null);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };
            var volume = input.Select(c => c.Volume).ToArray();

            return TANet.Util.StaticClasses.Indicators.Vwma(values, volume, period, signalLogic);
        }

        #endregion

        #region WMA

        public static MovingAverageResult Wma(double[] input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Wma, period);
        }
        public static MovingAverageResult Wma(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Wma, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Wma(List<Candle> input, int period)
        {
            return Indicators.MA(input, MovingAverageType.Wma, period);
        }
        public static MovingAverageResult Wma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Wma, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Wma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Indicators.MA(input, MovingAverageType.Wma, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Wma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Indicators.MA(input, MovingAverageType.Wma, period, calculationBase: calculationBase, signalLogic: signalLogic);
        }

        #endregion

        #region ZLEMA

        public static MovingAverageResult Zlema(double[] input, int period)
        {
            return TANet.Util.StaticClasses.Indicators.Zlema(input, period);
        }
        public static MovingAverageResult Zlema(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return TANet.Util.StaticClasses.Indicators.Zlema(input, period, signalLogic);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period)
        {
            return Zlema(input, period, IndicatorCalculationBase.Close, null);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return Zlema(input, period, IndicatorCalculationBase.Close, signalLogic);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return Zlema(input, period, calculationBase, null);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            var values = calculationBase switch
            {
                IndicatorCalculationBase.Open => input.Select(c => c.Open).ToArray(),
                IndicatorCalculationBase.High => input.Select(c => c.High).ToArray(),
                IndicatorCalculationBase.Low => input.Select(c => c.Low).ToArray(),
                IndicatorCalculationBase.Volume => input.Select(c => c.Volume).ToArray(),
                _ => input.Select(c => c.Close).ToArray()
            };

            return TANet.Util.StaticClasses.Indicators.Zlema(values, period, signalLogic);
        }

        #endregion

        #region Private Methods

        /* AROON */

        private static AroonResult AROON(List<Candle> candles,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();            

            return AROON(high, low, period, signalLogic);
        }
        private static AroonResult AROON(double[] high,
            double[] low,            
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Aroon(high, low, period, signalLogic);
        }

        /* AROON_OSC */

        private static AroonOscillatorResult AROON_OSC(List<Candle> candles,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();

            return AROON_OSC(high, low, period, signalLogic);
        }
        private static AroonOscillatorResult AROON_OSC(double[] high,
            double[] low,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.AroonOscillator(high, low, period, signalLogic);
        }

        /* ATR */

        private static AtrResult ATR(List<Candle> candles,
            int period,            
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            var close = candles.Select(c => c.Close).ToArray();

            return ATR(high, low, close, period, signalLogic);
        }
        private static AtrResult ATR(double[] high,
            double[] low,
            double[] close,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Atr(high, low, close, period, signalLogic);
        }

        /* B_BANDS */

        private static BollingerBandsResult B_BANDS(List<Candle> candles,            
            int period,
            double stdDevUp,
            double stdDevDown,
            MovingAverageType maType, 
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            double[] input;
            if (calculationBase == IndicatorCalculationBase.Close)
                input = candles.Select(c => c.Close).ToArray();

            else if (calculationBase == IndicatorCalculationBase.Open)
                input = candles.Select(c => c.Open).ToArray();

            else if (calculationBase == IndicatorCalculationBase.High)
                input = candles.Select(c => c.High).ToArray();

            else if (calculationBase == IndicatorCalculationBase.Low)
                input = candles.Select(c => c.Low).ToArray();

            else
                input = candles.Select(c => c.Close).ToArray();

            return B_BANDS(input, period, stdDevUp, stdDevDown, maType, signalLogic);
        }
        private static BollingerBandsResult B_BANDS(double[] input,
            int period,
            double stdDevUp,
            double stdDevDown,
            MovingAverageType maType,          
            Func<double[], double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.BollingerBands(input, period, stdDevUp, stdDevDown, maType, signalLogic);
        }

        /* CCI */

        private static CciResult CCI(List<Candle> candles,
            int period,            
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            var close = candles.Select(c => c.Close).ToArray();            

            return CCI(high, low, close, period, signalLogic);
        }
        private static CciResult CCI(double[] high,
            double[] low,
            double[] close,            
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Cci(high, low, close, period, signalLogic);
        }

        /* MACD */

        private static MacdResult MACD(List<Candle> candles, 
            MovingAverageType fastMaType, 
            int fastPeriod, 
            MovingAverageType slowMaType, 
            int slowPeriod, 
            MovingAverageType signalMaType, 
            int signalPeriod, 
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            double[] input;
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

            return MACD(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, signalLogic);
        }
        private static MacdResult MACD(double[] input, 
            MovingAverageType fastMaType, 
            int fastPeriod, 
            MovingAverageType slowMaType, 
            int slowPeriod, 
            MovingAverageType signalMaType, 
            int signalPeriod,
            Func<double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.MacdExt(input, fastMaType, fastPeriod, slowMaType, slowPeriod, signalMaType, signalPeriod, signalLogic);
        }

        /* MA */

        private static MovingAverageResult MA(List<Candle> candles, 
            MovingAverageType maType, 
            int period, 
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            double[] input;
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

            return MA(input, maType, period, signalLogic);
        }
        private static MovingAverageResult MA(double[] input, 
            MovingAverageType maType, 
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Ma(input, maType, period, signalLogic);
        }

        /* MFI */

        private static MfiResult MFI(List<Candle> candles,
            int period,            
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            var close = candles.Select(c => c.Close).ToArray();
            var volume = candles.Select(c => c.Volume).ToArray();

            return MFI(high, low, close, volume, period, signalLogic);
        }
        private static MfiResult MFI(double[] high,
            double[] low,
            double[] close,
            double[] volume,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Mfi(high, low, close, volume, period, signalLogic);
        }

        /* P_SAR */

        private static ParabolicSarResult P_SAR(List<Candle> candles,
            double acceleration,
            double maximum,
            Func<double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();

            return P_SAR(high, low, acceleration, maximum, signalLogic);
        }
        private static ParabolicSarResult P_SAR(double[] high,
            double[] low,
            double acceleration,
            double maximum,            
            Func<double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.ParabolicSar(high, low, acceleration, maximum, signalLogic);
        }

        /* RSI */

        private static RsiResult RSI(List<Candle> candles, 
            int period, 
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close, 
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            double[] input;
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

            return RSI(input, period, signalLogic);
        }
        private static RsiResult RSI(double[] input,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Rsi(input, period, signalLogic);
        }

        /* STOCH */

        private static StochasticResult STOCH(List<Candle> candles,
            int fastKPeriod,
            MovingAverageType slowKMaType,
            int slowKPeriod,
            MovingAverageType slowDMaType,
            int slowDPeriod,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {       
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            var close = candles.Select(c => c.Close).ToArray();

            return STOCH(high, low, close, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod, signalLogic);
        }
        private static StochasticResult STOCH(double[] high,
            double[] low, 
            double[] close,
            int fastKPeriod,
            MovingAverageType slowKMaType,
            int slowKPeriod,
            MovingAverageType slowDMaType,
            int slowDPeriod,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Stochastic(high, 
                low, close, fastKPeriod, slowKMaType, slowKPeriod, slowDMaType, slowDPeriod, signalLogic);
        }

        /* WILL_R */

        private static WilliamsRResult WILL_R(List<Candle> candles,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            var close = candles.Select(c => c.Close).ToArray();            

            return WILL_R(high, low, close, period, signalLogic);
        }
        private static WilliamsRResult WILL_R(double[] high,
            double[] low,
            double[] close,            
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.WilliamsR(high, low, close, period, signalLogic);
        }

        #endregion
    }
}
