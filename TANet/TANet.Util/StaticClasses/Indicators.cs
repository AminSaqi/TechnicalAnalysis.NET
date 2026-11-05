using System;
using TANet.Contracts.Enums;
using TANet.Contracts.OperationResults.Indicators;
using TANet.Util.StaticClasses.EnumConvertors;
using TANet.Util.Extensions;
using TicTacTec.TA.Library;

namespace TANet.Util.StaticClasses
{
    public static class Indicators
    {
        #region helper methods

        static double[] CopyToChronologicalOrder(double[] input)
        {
            var chronological = new double[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                chronological[i] = input[input.Length - 1 - i];
            }

            return chronological;
        }

        static void CompleteNewestFirstOutput(double[] output, int writtenCount)
        {
            Array.Reverse(output, 0, writtenCount);

            Array.Fill(
                output,
                double.NaN,
                writtenCount,
                output.Length - writtenCount);
        }

        #endregion

        #region Default Signal Logics

        static Func<double[], IndicatorSignal> aroonOscDefaultSignalLogic = output =>
            output[0] > 0 ? IndicatorSignal.Buy :
            output[0] < 0 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], IndicatorSignal> aroonDefaultSignalLogic = (outputAroonUp, outputAroonDown) =>
            outputAroonUp[0] > 50 && outputAroonDown[0] < 50 ? IndicatorSignal.Buy :
            outputAroonUp[0] < 50 && outputAroonDown[0] > 50 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> cciDefaultSignalLogic = output =>
            output[0] > 100 ? IndicatorSignal.Buy :
            output[0] < -100 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], double[], IndicatorSignal> macdDefaultSignalLogic = (outputMacd, outputSignal, outputHistogram) =>
            outputMacd[0] > 0 ? IndicatorSignal.Buy :
            outputMacd[0] < 0 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], IndicatorSignal> maDefaultSignalLogic = (input, output) =>
            input[0] > output[0] ? IndicatorSignal.Buy :
            input[0] < output[0] ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> mfiDefaultSignalLogic = output =>
            output[0] < 20 ? IndicatorSignal.Buy :
            output[0] > 80 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], double[], IndicatorSignal> pSarDefaultSignalLogic = (high, low, output) =>
            low[low.Length - 1] > output[output.Length - 1] ? IndicatorSignal.Buy :
            high[high.Length - 1] < output[output.Length - 1] ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> rsiDefaultSignalLogic = output =>
            output[0] <= 30 ? IndicatorSignal.Buy :
            output[0] >= 70 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], IndicatorSignal> stochDefaultSignalLogic = (slowK, slowD) =>
            slowK[slowK.Length - 1] < 20 && slowD[slowD.Length -1] < 20 ? IndicatorSignal.Buy :
            slowK[slowK.Length - 1] > 80 && slowD[slowD.Length - 1] > 80 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> williamsRDefaultSignalLogic = output =>
            output[output.Length - 1] < -80 ? IndicatorSignal.Buy :
            output[output.Length - 1] > -20 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        #endregion

        public static AroonResult Aroon(
            double[] inputHigh,
            double[] inputLow,
            int period,
            Func<double[], double[], IndicatorSignal> aroonSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(inputHigh);
                ArgumentNullException.ThrowIfNull(inputLow);

                if (inputHigh.Length != inputLow.Length)
                {
                    throw new ArgumentException("High and low series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(inputHigh);
                var chronologicalLow = CopyToChronologicalOrder(inputLow);
                var outputUp = new double[inputHigh.Length];
                var outputDown = new double[inputHigh.Length];

                var range = TALib.Core.Default.Aroon(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    period,
                    outputDown,
                    outputUp);

                CompleteNewestFirstOutput(outputUp, range.Count);
                CompleteNewestFirstOutput(outputDown, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (outputUp.HasValidData() && outputDown.HasValidData())
                {
                    indicatorSignal = aroonSignalLogic != null
                        ? aroonSignalLogic.Invoke(outputUp, outputDown)
                        : aroonDefaultSignalLogic.Invoke(outputUp, outputDown);
                }

                return new AroonResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    AroonUp = outputUp,
                    AroonDown = outputDown
                };
            }
            catch (Exception ex)
            {
                return new AroonResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static AroonOscillatorResult AroonOscillator(
            double[] inputHigh,
            double[] inputLow,
            int period,
            Func<double[], IndicatorSignal> aroonOscSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(inputHigh);
                ArgumentNullException.ThrowIfNull(inputLow);

                if (inputHigh.Length != inputLow.Length)
                {
                    throw new ArgumentException("High and low series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(inputHigh);
                var chronologicalLow = CopyToChronologicalOrder(inputLow);
                var output = new double[inputHigh.Length];

                var range = TALib.Core.Default.Aroonosc(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = aroonOscSignalLogic != null
                        ? aroonOscSignalLogic.Invoke(output)
                        : aroonOscDefaultSignalLogic.Invoke(output);
                }

                return new AroonOscillatorResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    AroonOscillator = output
                };
            }
            catch (Exception ex)
            {
                return new AroonOscillatorResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static AtrResult Atr(
            double[] inputHigh,
            double[] inputLow,
            double[] inputClose,
            int period,
            Func<double[], IndicatorSignal> atrSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(inputHigh);
                ArgumentNullException.ThrowIfNull(inputLow);
                ArgumentNullException.ThrowIfNull(inputClose);

                if (inputHigh.Length != inputLow.Length || inputHigh.Length != inputClose.Length)
                {
                    throw new ArgumentException("High, low and close series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(inputHigh);
                var chronologicalLow = CopyToChronologicalOrder(inputLow);
                var chronologicalClose = CopyToChronologicalOrder(inputClose);
                var output = new double[inputHigh.Length];

                var range = TALib.Core.Default.Atr(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    chronologicalClose,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData() && atrSignalLogic != null)
                {
                    indicatorSignal = atrSignalLogic.Invoke(output);
                }

                return new AtrResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Atr = output
                };
            }
            catch (Exception ex)
            {
                return new AtrResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static BollingerBandsResult BollingerBands(
            double[] input,
            int period,
            double stdDevUp,
            double stdDevDown,
            MovingAverageType maType,            
            Func<double[], double[], double[], double[], IndicatorSignal> bBandsSignalLogic = null)
        {
            try
            {       
                var chronologicalInput = CopyToChronologicalOrder(input);
                var outputUp = new double[input.Length];
                var outputMiddle = new double[input.Length];
                var outputDown = new double[input.Length];

                var range = TALib.Core.Default.Bbands(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    stdDevUp,
                    stdDevDown,
                    MovingAverageTypes.ToOfficialTaLib(maType),
                    outputUp,
                    outputMiddle,
                    outputDown);

                CompleteNewestFirstOutput(outputUp, range.Count);
                CompleteNewestFirstOutput(outputMiddle, range.Count);
                CompleteNewestFirstOutput(outputDown, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;

                if (outputUp.HasValidData() && outputMiddle.HasValidData() && outputDown.HasValidData())
                {
                    indicatorSignal = bBandsSignalLogic != null ?
                        bBandsSignalLogic.Invoke(input, outputUp, outputMiddle, outputDown) :
                        IndicatorSignal.Stay;
                }

                return new BollingerBandsResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    UpperBand = outputUp,
                    MiddleBand = outputMiddle,
                    LowerBand = outputDown
                };
            }
            catch (Exception ex)
            {
                return new BollingerBandsResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static CciResult Cci(
            double[] inputHigh,
            double[] inputLow,
            double[] inputClose,
            int period,
            Func<double[], IndicatorSignal> cciSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(inputHigh);
                ArgumentNullException.ThrowIfNull(inputLow);
                ArgumentNullException.ThrowIfNull(inputClose);

                if (inputHigh.Length != inputLow.Length || inputHigh.Length != inputClose.Length)
                {
                    throw new ArgumentException("High, low and close series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(inputHigh);
                var chronologicalLow = CopyToChronologicalOrder(inputLow);
                var chronologicalClose = CopyToChronologicalOrder(inputClose);
                var output = new double[inputHigh.Length];

                var range = TALib.Core.Default.Cci(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    chronologicalClose,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = cciSignalLogic != null
                        ? cciSignalLogic.Invoke(output)
                        : cciDefaultSignalLogic.Invoke(output);
                }

                return new CciResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Cci = output
                };
            }
            catch (Exception ex)
            {
                return new CciResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static MovingAverageResult Ma(
            double[] input, 
            MovingAverageType maType, 
            int period, 
            Func<double[], double[], IndicatorSignal> maSignalLogic = null)
        {
            try
            {        
                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Ma(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    MovingAverageTypes.ToOfficialTaLib(maType),
                    output);                

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;

                if (output.HasValidData())
                {
                    indicatorSignal = maSignalLogic != null ?
                        maSignalLogic.Invoke(input, output) :
                        maDefaultSignalLogic.Invoke(input, output);
                }

                return new MovingAverageResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Ma = output
                };

            }
            catch (Exception ex)
            {
                return new MovingAverageResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }        

        public static MacdResult MacdExt(
            double[] input, 
            MovingAverageType fastMaType, 
            int fastPeriod, 
            MovingAverageType slowMaType, 
            int slowPeriod, 
            MovingAverageType signalMaType, 
            int signalPeriod, 
            Func<double[], double[], double[], IndicatorSignal> macdExtSignalLogic = null)
        {
            try
            {
                var chronologicalInput = CopyToChronologicalOrder(input);
                var outputMacd = new double[input.Length];
                var outputSignal = new double[input.Length];
                var outputHistogram = new double[input.Length];

                var range = TALib.Core.Default.Macdext(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    fastPeriod,
                    MovingAverageTypes.ToOfficialTaLib(fastMaType),
                    slowPeriod,
                    MovingAverageTypes.ToOfficialTaLib(slowMaType),
                    signalPeriod,
                    MovingAverageTypes.ToOfficialTaLib(signalMaType),
                    outputMacd,
                    outputSignal,
                    outputHistogram);

                CompleteNewestFirstOutput(outputMacd, range.Count);
                CompleteNewestFirstOutput(outputSignal, range.Count);
                CompleteNewestFirstOutput(outputHistogram, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (outputMacd.HasValidData() && outputSignal.HasValidData() && outputHistogram.HasValidData())
                {
                    indicatorSignal = macdExtSignalLogic != null
                        ? macdExtSignalLogic.Invoke(outputMacd, outputSignal, outputHistogram)
                        : macdDefaultSignalLogic.Invoke(outputMacd, outputSignal, outputHistogram);
                }

                return new MacdResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Macd = outputMacd,
                    Signal = outputSignal,
                    Histogram = outputHistogram
                };
            }
            catch (Exception ex)
            {
                return new MacdResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static MfiResult Mfi(double[] inputHigh,
            double[] inputLow,
            double[] inputClose,
            double[] inputVolume,
            int period,
            Func<double[], IndicatorSignal> mfiSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(inputHigh);
                ArgumentNullException.ThrowIfNull(inputLow);
                ArgumentNullException.ThrowIfNull(inputClose);
                ArgumentNullException.ThrowIfNull(inputVolume);

                if (inputHigh.Length != inputLow.Length ||
                    inputHigh.Length != inputClose.Length ||
                    inputHigh.Length != inputVolume.Length)
                {
                    throw new ArgumentException("High, low, close and volume series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(inputHigh);
                var chronologicalLow = CopyToChronologicalOrder(inputLow);
                var chronologicalClose = CopyToChronologicalOrder(inputClose);
                var chronologicalVolume = CopyToChronologicalOrder(inputVolume);
                var output = new double[inputHigh.Length];

                var range = TALib.Core.Default.Mfi(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    chronologicalClose,
                    chronologicalVolume,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = mfiSignalLogic != null
                        ? mfiSignalLogic.Invoke(output)
                        : mfiDefaultSignalLogic.Invoke(output);
                }

                return new MfiResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Mfi = output
                };
            }
            catch (Exception ex)
            {
                return new MfiResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static ParabolicSarResult ParabolicSar(double[] high,
            double[] low,
            double acceleration,
            double maximum,
            Func<double[], double[], double[], IndicatorSignal> pSarSignalLogic = null)
        {
            try
            {
                double[] output = new double[high.Length];
                var result = Core.Sar(0,
                    high.Length - 1,
                    high, 
                    low, 
                    acceleration,
                    maximum,
                    out int outBeginIndex,
                    out int outElementsCount,
                    output);

                if (result == Core.RetCode.Success)
                {
                    var outputValues = new double[outElementsCount];

                    Array.Reverse(output);
                    Array.ConstrainedCopy(output, outBeginIndex, outputValues, 0, outElementsCount);
                    Array.Reverse(outputValues);

                    return new ParabolicSarResult
                    {
                        Success = true,
                        IndicatorSignal = pSarSignalLogic != null ?
                            pSarSignalLogic.Invoke(high, low, outputValues) : pSarDefaultSignalLogic.Invoke(high, low, outputValues),
                        Sar = outputValues
                    };
                }
                else
                {
                    return new ParabolicSarResult
                    {
                        Success = false,
                        IndicatorSignal = IndicatorSignal.Stay,
                        Message = result.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ParabolicSarResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static RsiResult Rsi(double[] input, int period, Func<double[], IndicatorSignal> rsiSignalLogic = null)
        {
            try
            {
                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Rsi(0,
                    chronologicalInput.Length - 1,
                    chronologicalInput, 
                    period, 
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;

                if (output.HasValidData())
                {
                    indicatorSignal = rsiSignalLogic != null ?
                        rsiSignalLogic.Invoke(output) :
                        rsiDefaultSignalLogic.Invoke(output);
                }

                return new RsiResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Rsi = output
                };                
            }
            catch (Exception ex)
            {
                return new RsiResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static StochasticResult Stochastic(double[] inputHigh, 
            double[] inputLow, 
            double[] inputClose, 
            int fastKPeriod, 
            MovingAverageType slowKMaType, 
            int slowKPeriod, 
            MovingAverageType slowDMaType, 
            int slowDPeriod, 
            Func<double[], double[], IndicatorSignal> stochSignalLogic = null)
        {            
            try
            {
                var indicatorSignal = IndicatorSignal.Stay;

                double[] outputSlowK = new double[inputHigh.Length];
                double[] outputSlowD = new double[inputHigh.Length];

                var result = Core.Stoch(0, 
                    inputHigh.Length - 1,
                    inputHigh, 
                    inputLow, 
                    inputClose, 
                    fastKPeriod, 
                    slowKPeriod,
                    MovingAverageTypes.ToTaLib(slowKMaType), 
                    slowDPeriod,
                    MovingAverageTypes.ToTaLib(slowDMaType), 
                    out int outBeginIndex,
                    out int outElementsCount, 
                    outputSlowK, 
                    outputSlowD);

                if (result == Core.RetCode.Success)
                {
                    var outputSlowKValues = new double[outElementsCount];
                    var outputSlowDValues = new double[outElementsCount];                    

                    Array.Reverse(outputSlowK);
                    Array.Reverse(outputSlowD);                    

                    Array.ConstrainedCopy(outputSlowK, outBeginIndex, outputSlowKValues, 0, outElementsCount);
                    Array.ConstrainedCopy(outputSlowD, outBeginIndex, outputSlowDValues, 0, outElementsCount);                    

                    Array.Reverse(outputSlowKValues);
                    Array.Reverse(outputSlowDValues);

                    indicatorSignal = stochSignalLogic != null ?
                            stochSignalLogic.Invoke(outputSlowKValues, outputSlowDValues) :
                            stochDefaultSignalLogic.Invoke(outputSlowKValues, outputSlowDValues);

                    return new StochasticResult
                    {
                        Success = true,
                        IndicatorSignal = indicatorSignal,
                        SlowK = outputSlowKValues,
                        SlowD = outputSlowDValues
                    };
                }
                else
                {
                    return new StochasticResult
                    {
                        Success = false,
                        IndicatorSignal = IndicatorSignal.Stay,
                        Message = result.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new StochasticResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static WilliamsRResult WilliamsR(double[] inputHigh,
            double[] inputLow,
            double[] inputClose,
            int period,
            Func<double[], IndicatorSignal> williamsRSignalLogic = null)
        {
            try
            {
                var indicatorSignal = IndicatorSignal.Stay;

                double[] output = new double[inputHigh.Length];

                var result = Core.WillR(0,
                    inputHigh.Length - 1,
                    inputHigh, 
                    inputLow, 
                    inputClose, 
                    period,
                    out int outBeginIndex,
                    out int outElementsCount,
                    output);

                if (result == Core.RetCode.Success)
                {
                    var outputValues = new double[outElementsCount];

                    Array.Reverse(outputValues);
                    Array.ConstrainedCopy(output, outBeginIndex, outputValues, 0, outElementsCount);
                    Array.Reverse(outputValues);

                    indicatorSignal = williamsRSignalLogic != null ?
                            williamsRSignalLogic.Invoke(outputValues) :
                            williamsRDefaultSignalLogic.Invoke(outputValues);

                    return new WilliamsRResult
                    {
                        Success = true,
                        IndicatorSignal = indicatorSignal,
                        RangePercent = outputValues
                    };
                }
                else
                {
                    return new WilliamsRResult
                    {
                        Success = false,
                        IndicatorSignal = IndicatorSignal.Stay,
                        Message = result.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new WilliamsRResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }
    }
}
