using System;
using TANet.Contracts.Enums;
using TANet.Contracts.OperationResults.Indicators;
using TANet.Util.StaticClasses.EnumConvertors;
using TANet.Util.Extensions;

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

        static Func<double[], double[], IndicatorSignal> mamaDefaultSignalLogic = (mama, fama) =>
            mama[0] > fama[0] ? IndicatorSignal.Buy :
            mama[0] < fama[0] ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> mfiDefaultSignalLogic = output =>
            output[0] < 20 ? IndicatorSignal.Buy :
            output[0] > 80 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> momDefaultSignalLogic = output =>
            output[0] > 0 ? IndicatorSignal.Buy :
            output[0] < 0 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], double[], IndicatorSignal> pSarDefaultSignalLogic = (high, low, output) =>
            low[0] > output[0] ? IndicatorSignal.Buy :
            high[0] < output[0] ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> rsiDefaultSignalLogic = output =>
            output[0] <= 30 ? IndicatorSignal.Buy :
            output[0] >= 70 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], double[], IndicatorSignal> stochDefaultSignalLogic = (slowK, slowD) =>
            slowK[0] < 20 && slowD[0] < 20 ? IndicatorSignal.Buy :
            slowK[0] > 80 && slowD[0] > 80 ? IndicatorSignal.Sell :
            IndicatorSignal.Stay;

        static Func<double[], IndicatorSignal> williamsRDefaultSignalLogic = output =>
            output[0] < -80 ? IndicatorSignal.Buy :
            output[0] > -20 ? IndicatorSignal.Sell :
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

        public static DonchianResult Donchian(
            double[] high,
            double[] low,
            int period,
            Func<double[], double[], double[], double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(high);
                ArgumentNullException.ThrowIfNull(low);

                if (high.Length != low.Length)
                {
                    throw new ArgumentException("High and low series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(high);
                var chronologicalLow = CopyToChronologicalOrder(low);
                var upper = new double[high.Length];
                var middle = new double[high.Length];
                var lower = new double[high.Length];

                var range = TALib.Core.Default.Donchian(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    period,
                    upper,
                    middle,
                    lower);

                CompleteNewestFirstOutput(upper, range.Count);
                CompleteNewestFirstOutput(middle, range.Count);
                CompleteNewestFirstOutput(lower, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (signalLogic != null && upper.HasValidData() && middle.HasValidData() && lower.HasValidData())
                {
                    indicatorSignal = signalLogic.Invoke(high, low, upper, middle, lower);
                }

                return new DonchianResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    UpperBand = upper,
                    MiddleBand = middle,
                    LowerBand = lower
                };
            }
            catch (Exception ex)
            {
                return new DonchianResult
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

        public static MovingAverageResult Alma(
            double[] input,
            int period,
            double sigma,
            double offset,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Alma(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    sigma,
                    offset,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult Zlema(
            double[] input,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Zlema(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult Dema(
            double[] input,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Dema(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MamaResult Mama(
            double[] input,
            double fastLimit,
            double slowLimit,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var outputMama = new double[input.Length];
                var outputFama = new double[input.Length];

                var range = TALib.Core.Default.Mama(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    fastLimit,
                    slowLimit,
                    outputMama,
                    outputFama);

                CompleteNewestFirstOutput(outputMama, range.Count);
                CompleteNewestFirstOutput(outputFama, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (outputMama.HasValidData() && outputFama.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(outputMama, outputFama)
                        : mamaDefaultSignalLogic.Invoke(outputMama, outputFama);
                }

                return new MamaResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Mama = outputMama,
                    Fama = outputFama
                };
            }
            catch (Exception ex)
            {
                return new MamaResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static MovingAverageResult Hma(
            double[] input,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Hma(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult Rma(
            double[] input,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Rma(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult Trima(
            double[] input,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Trima(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult Vidya(
            double[] input,
            int period,
            int cmoPeriod,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Vidya(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    cmoPeriod,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult Vwma(
            double[] input,
            double[] volume,
            int period,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);
                ArgumentNullException.ThrowIfNull(volume);

                if (input.Length != volume.Length)
                {
                    throw new ArgumentException("Input and volume series must have the same length.");
                }

                var chronologicalInput = CopyToChronologicalOrder(input);
                var chronologicalVolume = CopyToChronologicalOrder(volume);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Vwma(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    chronologicalVolume,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MovingAverageResult T3(
            double[] input,
            int period,
            double vFactor,
            Func<double[], double[], IndicatorSignal> signalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);

                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.T3(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    vFactor,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = signalLogic != null
                        ? signalLogic.Invoke(input, output)
                        : maDefaultSignalLogic.Invoke(input, output);
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

        public static MomResult Mom(double[] input, int period, Func<double[], IndicatorSignal> momSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(input);
                var chronologicalInput = CopyToChronologicalOrder(input);
                var output = new double[input.Length];

                var range = TALib.Core.Default.Mom(
                    0,
                    chronologicalInput.Length - 1,
                    chronologicalInput,
                    period,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = momSignalLogic != null
                        ? momSignalLogic.Invoke(output)
                        : momDefaultSignalLogic.Invoke(output);
                }

                return new MomResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Mom = output
                };
            }
            catch (Exception ex)
            {
                return new MomResult
                {
                    Success = false,
                    IndicatorSignal = IndicatorSignal.Stay,
                    Message = ex.ToString()
                };
            }
        }

        public static ParabolicSarResult ParabolicSar(
            double[] high,
            double[] low,
            double acceleration,
            double maximum,
            Func<double[], double[], double[], IndicatorSignal> pSarSignalLogic = null)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(high);
                ArgumentNullException.ThrowIfNull(low);

                if (high.Length != low.Length)
                {
                    throw new ArgumentException("High and low series must have the same length.");
                }

                var chronologicalHigh = CopyToChronologicalOrder(high);
                var chronologicalLow = CopyToChronologicalOrder(low);
                var output = new double[high.Length];

                var range = TALib.Core.Default.Sar(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    acceleration,
                    maximum,
                    output);

                CompleteNewestFirstOutput(output, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (output.HasValidData())
                {
                    indicatorSignal = pSarSignalLogic != null
                        ? pSarSignalLogic.Invoke(high, low, output)
                        : pSarDefaultSignalLogic.Invoke(high, low, output);
                }

                return new ParabolicSarResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    Sar = output
                };
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

        public static StochasticResult Stochastic(
            double[] inputHigh,
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
                var outputSlowK = new double[inputHigh.Length];
                var outputSlowD = new double[inputHigh.Length];

                var range = TALib.Core.Default.Stoch(
                    0,
                    chronologicalHigh.Length - 1,
                    chronologicalHigh,
                    chronologicalLow,
                    chronologicalClose,
                    fastKPeriod,
                    slowKPeriod,
                    MovingAverageTypes.ToOfficialTaLib(slowKMaType),
                    slowDPeriod,
                    MovingAverageTypes.ToOfficialTaLib(slowDMaType),
                    outputSlowK,
                    outputSlowD);

                CompleteNewestFirstOutput(outputSlowK, range.Count);
                CompleteNewestFirstOutput(outputSlowD, range.Count);

                var indicatorSignal = IndicatorSignal.Stay;
                if (outputSlowK.HasValidData() && outputSlowD.HasValidData())
                {
                    indicatorSignal = stochSignalLogic != null
                        ? stochSignalLogic.Invoke(outputSlowK, outputSlowD)
                        : stochDefaultSignalLogic.Invoke(outputSlowK, outputSlowD);
                }

                return new StochasticResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    SlowK = outputSlowK,
                    SlowD = outputSlowD
                };
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

        public static WilliamsRResult WilliamsR(
            double[] inputHigh,
            double[] inputLow,
            double[] inputClose,
            int period,
            Func<double[], IndicatorSignal> williamsRSignalLogic = null)
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

                var range = TALib.Core.Default.Willr(
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
                    indicatorSignal = williamsRSignalLogic != null
                        ? williamsRSignalLogic.Invoke(output)
                        : williamsRDefaultSignalLogic.Invoke(output);
                }

                return new WilliamsRResult
                {
                    Success = true,
                    IndicatorSignal = indicatorSignal,
                    RangePercent = output
                };
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
