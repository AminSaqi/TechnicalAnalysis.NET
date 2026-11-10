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
            return ALMA(input, period, sigma, offset);
        }
        public static MovingAverageResult Alma(double[] input, int period, double sigma, double offset, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return ALMA(input, period, sigma, offset, signalLogic);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset)
        {
            return ALMA(input, period, sigma, offset);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return ALMA(input, period, sigma, offset, signalLogic: signalLogic);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset, IndicatorCalculationBase calculationBase)
        {
            return ALMA(input, period, sigma, offset, calculationBase: calculationBase);
        }
        public static MovingAverageResult Alma(List<Candle> input, int period, double sigma, double offset, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return ALMA(input, period, sigma, offset, calculationBase, signalLogic);
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
            return MA(input, MovingAverageType.Dema, period);
        }
        public static MovingAverageResult Dema(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Dema, period, signalLogic);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period)
        {
            return MA(input, MovingAverageType.Dema, period);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Dema, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return MA(input, MovingAverageType.Dema, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Dema(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Dema, period, calculationBase, signalLogic);
        }

        #endregion

        #region Donchian

        public static DonchianResult Donchian(double[] high, double[] low, int period)
        {
            return DONCHIAN(high, low, period);
        }
        public static DonchianResult Donchian(double[] high, double[] low, int period, Func<double[], double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return DONCHIAN(high, low, period, signalLogic);
        }
        public static DonchianResult Donchian(List<Candle> candles, int period)
        {
            return DONCHIAN(candles, period);
        }
        public static DonchianResult Donchian(List<Candle> candles, int period, Func<double[], double[], double[], double[], double[], IndicatorSignal> signalLogic)
        {
            return DONCHIAN(candles, period, signalLogic);
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
            return MA(input, MovingAverageType.Hma, period);
        }
        public static MovingAverageResult Hma(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Hma, period, signalLogic);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period)
        {
            return MA(input, MovingAverageType.Hma, period);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Hma, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return MA(input, MovingAverageType.Hma, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Hma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Hma, period, calculationBase, signalLogic);
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

        #region MA

        public static MovingAverageResult Ma(double[] input, MovingAverageType maType, int period)
        {
            return MA(input, maType, period);
        }
        public static MovingAverageResult Ma(double[] input, MovingAverageType maType, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, maType, period, signalLogic);
        }
        public static MovingAverageResult Ma(List<Candle> input, MovingAverageType maType, int period)
        {
            return MA(input, maType, period);
        }
        public static MovingAverageResult Ma(List<Candle> input, MovingAverageType maType, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, maType, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Ma(List<Candle> input, MovingAverageType maType, int period, IndicatorCalculationBase calculationBase)
        {
            return MA(input, maType, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Ma(List<Candle> input, MovingAverageType maType, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, maType, period, calculationBase, signalLogic);
        }

        #endregion

        #region MAMA

        public static MamaResult Mama(double[] input, double fastLimit, double slowLimit)
        {
            return MAMA(input, fastLimit, slowLimit);
        }
        public static MamaResult Mama(double[] input, double fastLimit, double slowLimit, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MAMA(input, fastLimit, slowLimit, signalLogic);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit)
        {
            return MAMA(input, fastLimit, slowLimit);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MAMA(input, fastLimit, slowLimit, signalLogic: signalLogic);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit, IndicatorCalculationBase calculationBase)
        {
            return MAMA(input, fastLimit, slowLimit, calculationBase: calculationBase);
        }
        public static MamaResult Mama(List<Candle> input, double fastLimit, double slowLimit, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MAMA(input, fastLimit, slowLimit, calculationBase, signalLogic);
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

        #region MOM

        public static MomResult Mom(double[] input, int period)
        {
            return MOM(input, period);
        }
        public static MomResult Mom(double[] input, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return MOM(input, period, signalLogic);
        }
        public static MomResult Mom(List<Candle> input, int period)
        {
            return MOM(input, period);
        }
        public static MomResult Mom(List<Candle> input, int period, Func<double[], IndicatorSignal> signalLogic)
        {
            return MOM(input, period, signalLogic: signalLogic);
        }
        public static MomResult Mom(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return MOM(input, period, calculationBase);
        }
        public static MomResult Mom(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], IndicatorSignal> signalLogic)
        {
            return MOM(input, period, calculationBase, signalLogic);
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
            return MA(input, MovingAverageType.Rma, period);
        }
        public static MovingAverageResult Rma(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Rma, period, signalLogic);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period)
        {
            return MA(input, MovingAverageType.Rma, period);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Rma, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return MA(input, MovingAverageType.Rma, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Rma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Rma, period, calculationBase, signalLogic);
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
            return T3Core(input, period, vFactor);
        }
        public static MovingAverageResult T3(double[] input, int period, double vFactor, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return T3Core(input, period, vFactor, signalLogic);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor)
        {
            return T3Core(input, period, vFactor);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return T3Core(input, period, vFactor, signalLogic: signalLogic);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor, IndicatorCalculationBase calculationBase)
        {
            return T3Core(input, period, vFactor, calculationBase: calculationBase);
        }
        public static MovingAverageResult T3(List<Candle> input, int period, double vFactor, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return T3Core(input, period, vFactor, calculationBase, signalLogic);
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
            return MA(input, MovingAverageType.Trima, period);
        }
        public static MovingAverageResult Trima(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Trima, period, signalLogic);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period)
        {
            return MA(input, MovingAverageType.Trima, period);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Trima, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return MA(input, MovingAverageType.Trima, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Trima(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Trima, period, calculationBase, signalLogic);
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

        #region VIDYA

        public static MovingAverageResult Vidya(double[] input, int period, int cmoPeriod)
        {
            return VIDYA(input, period, cmoPeriod);
        }
        public static MovingAverageResult Vidya(double[] input, int period, int cmoPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return VIDYA(input, period, cmoPeriod, signalLogic);
        }
        public static MovingAverageResult Vidya(List<Candle> input, int period, int cmoPeriod)
        {
            return VIDYA(input, period, cmoPeriod);
        }
        public static MovingAverageResult Vidya(List<Candle> input, int period, int cmoPeriod, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return VIDYA(input, period, cmoPeriod, signalLogic: signalLogic);
        }
        public static MovingAverageResult Vidya(List<Candle> input, int period, int cmoPeriod, IndicatorCalculationBase calculationBase)
        {
            return VIDYA(input, period, cmoPeriod, calculationBase: calculationBase);
        }
        public static MovingAverageResult Vidya(List<Candle> input, int period, int cmoPeriod, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return VIDYA(input, period, cmoPeriod, calculationBase, signalLogic);
        }

        #endregion

        #region VWMA

        public static MovingAverageResult Vwma(double[] input, double[] volume, int period)
        {
            return VWMA(input, volume, period);
        }
        public static MovingAverageResult Vwma(double[] input, double[] volume, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return VWMA(input, volume, period, signalLogic);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period)
        {
            return VWMA(input, period);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return VWMA(input, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return VWMA(input, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Vwma(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return VWMA(input, period, calculationBase, signalLogic);
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
            return MA(input, MovingAverageType.Zlema, period);
        }
        public static MovingAverageResult Zlema(double[] input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Zlema, period, signalLogic);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period)
        {
            return MA(input, MovingAverageType.Zlema, period);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Zlema, period, signalLogic: signalLogic);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period, IndicatorCalculationBase calculationBase)
        {
            return MA(input, MovingAverageType.Zlema, period, calculationBase: calculationBase);
        }
        public static MovingAverageResult Zlema(List<Candle> input, int period, IndicatorCalculationBase calculationBase, Func<double[], double[], IndicatorSignal> signalLogic)
        {
            return MA(input, MovingAverageType.Zlema, period, calculationBase, signalLogic);
        }

        #endregion

        #region Private Methods

        private static double[] GetValuesFromCandles(List<Candle> candles, IndicatorCalculationBase calculationBase)
        {
            return calculationBase switch
            {
                IndicatorCalculationBase.Open => [.. candles.Select(c => c.Open)],
                IndicatorCalculationBase.High => [.. candles.Select(c => c.High)],
                IndicatorCalculationBase.Low => [.. candles.Select(c => c.Low)],
                IndicatorCalculationBase.Volume => [.. candles.Select(c => c.Volume)],
                _ => [.. candles.Select(c => c.Close)]
            };
        }

        /* ALMA */

        private static MovingAverageResult ALMA(
            List<Candle> candles, 
            int period, 
            double sigma, 
            double offset,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            var input = GetValuesFromCandles(candles, calculationBase);

            return ALMA(input, period, sigma, offset, signalLogic);
        }

        private static MovingAverageResult ALMA(
            double[] input, 
            int period, 
            double sigma, 
            double offset,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Alma(input, period, sigma, offset, signalLogic);
        }

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
                input = [.. candles.Select(c => c.Close)];

            else if (calculationBase == IndicatorCalculationBase.Open)
                input = [.. candles.Select(c => c.Open)];

            else if (calculationBase == IndicatorCalculationBase.High)
                input = [.. candles.Select(c => c.High)];

            else if (calculationBase == IndicatorCalculationBase.Low)
                input = [.. candles.Select(c => c.Low)];

            else
                input = [.. candles.Select(c => c.Close)];

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

        /* DONCHIAN */

        private static DonchianResult DONCHIAN(
            List<Candle> candles,
            int period,
            Func<double[], double[], double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            return DONCHIAN(high, low, period, signalLogic);
        }

        private static DonchianResult DONCHIAN(
            double[] high,
            double[] low,
            int period,
            Func<double[], double[], double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Donchian(high, low, period, signalLogic);
        }

        /* MA */

        private static MovingAverageResult MA(List<Candle> candles,
            MovingAverageType maType,
            int period,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return MA(GetValuesFromCandles(candles, calculationBase), maType, period, signalLogic);
        }
        private static MovingAverageResult MA(double[] input, 
            MovingAverageType maType, 
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Ma(input, maType, period, signalLogic);
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
                input = [.. candles.Select(c => c.Close)];

            else if (calculationBase == IndicatorCalculationBase.Open)
                input = [.. candles.Select(c => c.Open)];

            else if (calculationBase == IndicatorCalculationBase.High)
                input = [.. candles.Select(c => c.High)];

            else if (calculationBase == IndicatorCalculationBase.Low)
                input = [.. candles.Select(c => c.Low)];

            else if (calculationBase == IndicatorCalculationBase.Volume)
                input = [.. candles.Select(c => c.Volume)];

            else
                input = [.. candles.Select(c => c.Close)];

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

        /* MAMA */

        private static MamaResult MAMA(
            List<Candle> candles, 
            double fastLimit, 
            double slowLimit,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            var input = GetValuesFromCandles(candles, calculationBase);

            return MAMA(input, fastLimit, slowLimit, signalLogic);
        }

        private static MamaResult MAMA(
            double[] input, 
            double fastLimit, 
            double slowLimit,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Mama(input, fastLimit, slowLimit, signalLogic);
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

        /* MOM */

        private static MomResult MOM(
            List<Candle> candles,
            int period,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return MOM(GetValuesFromCandles(candles, calculationBase), period, signalLogic);
        }

        private static MomResult MOM(double[] input, int period, Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Mom(input, period, signalLogic);
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

        private static RsiResult RSI(
            List<Candle> candles, 
            int period, 
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close, 
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            double[] input;
            if (calculationBase == IndicatorCalculationBase.Close)
                input = [.. candles.Select(c => c.Close)];

            else if (calculationBase == IndicatorCalculationBase.Open)
                input = [.. candles.Select(c => c.Open)];

            else if (calculationBase == IndicatorCalculationBase.High)
                input = [.. candles.Select(c => c.High)];

            else if (calculationBase == IndicatorCalculationBase.Low)
                input = [.. candles.Select(c => c.Low)];

            else if (calculationBase == IndicatorCalculationBase.Volume)
                input = [.. candles.Select(c => c.Volume)];

            else
                input = [.. candles.Select(c => c.Close)];

            return RSI(input, period, signalLogic);
        }
        private static RsiResult RSI(
            double[] input,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Rsi(input, period, signalLogic);
        }

        /* STOCH */

        private static StochasticResult STOCH(
            List<Candle> candles,
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

        /* T3Core */

        private static MovingAverageResult T3Core(
            List<Candle> candles, 
            int period, 
            double vFactor,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            var input = GetValuesFromCandles(candles, calculationBase);

            return T3Core(input, period, vFactor, signalLogic);
        }

        private static MovingAverageResult T3Core(
            double[] input, 
            int period, 
            double vFactor,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.T3(input, period, vFactor, signalLogic);
        }

        /* VIDYA */

        private static MovingAverageResult VIDYA(
            List<Candle> candles, 
            int period, 
            int cmoPeriod,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            var input = GetValuesFromCandles(candles, calculationBase);

            return VIDYA(input, period, cmoPeriod, signalLogic);
        }

        private static MovingAverageResult VIDYA(
            double[] input, 
            int period, 
            int cmoPeriod,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Vidya(input, period, cmoPeriod, signalLogic);
        }

        /* VWMA */

        private static MovingAverageResult VWMA(
            List<Candle> candles, 
            int period,
            IndicatorCalculationBase calculationBase = IndicatorCalculationBase.Close,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            var input = GetValuesFromCandles(candles, calculationBase);
            var volume = candles.Select(c => c.Volume).ToArray();

            return VWMA(input, volume, period, signalLogic);
        }

        private static MovingAverageResult VWMA(
            double[] input, 
            double[] volume, 
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            return TANet.Util.StaticClasses.Indicators.Vwma(input, volume, period, signalLogic);
        }

        /* WILL_R */

        private static WilliamsRResult WILL_R(
            List<Candle> candles,
            int period,
            Func<double[], IndicatorSignal> signalLogic = null)
        {
            var high = candles.Select(c => c.High).ToArray();
            var low = candles.Select(c => c.Low).ToArray();
            var close = candles.Select(c => c.Close).ToArray();            

            return WILL_R(high, low, close, period, signalLogic);
        }
        private static WilliamsRResult WILL_R(
            double[] high,
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
