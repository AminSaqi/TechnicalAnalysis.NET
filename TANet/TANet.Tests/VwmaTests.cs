using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class VwmaTests
{
    [Fact]
    public void Vwma_ReturnsKnownWeightedValuesInNewestFirstOrder()
    {
        double[] input = [50, 40, 30, 20, 10];
        double[] volume = [5, 4, 3, 2, 1];
        var originalInput = (double[])input.Clone();
        var originalVolume = (double[])volume.Clone();

        var result = Indicators.Vwma(input, volume, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(500d / 12, result.Ma[0], precision: 10);
        Assert.Equal(290d / 9, result.Ma[1], precision: 10);
        Assert.Equal(140d / 6, result.Ma[2], precision: 10);
        Assert.All(result.Ma[3..], value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(3, result.Ma.CountValidData());
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(originalInput, input);
        Assert.Equal(originalVolume, volume);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public void Vwma_MatchesOfficialTaLib(int period)
    {
        double[] input = [20, 18, 19, 15, 17, 14, 13];
        double[] volume = [3, 8, 1, 4, 5, 2, 6];

        var result = Indicators.Vwma(input, volume, period);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Vwma(
            0, input.Length - 1, input.Reverse().ToArray(), volume.Reverse().ToArray(), period, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Ma.CountValidData());
    }

    [Fact]
    public void Vwma_WithEqualVolumes_MatchesSimpleMovingAverage()
    {
        double[] input = [7, 5, 6, 4, 3, 2, 1];
        double[] volume = [2, 2, 2, 2, 2, 2, 2];

        var result = Indicators.Vwma(input, volume, period: 3);
        var sma = Indicators.Sma(input, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.True(sma.Success, sma.Message);
        Assert.Equal(sma.Ma, result.Ma);
    }

    [Fact]
    public void Vwma_WhenPeriodIsOne_ReturnsInputEvenWithZeroVolume()
    {
        double[] input = [5, 4, 3];
        double[] volume = [0, 1, 0];

        var result = Indicators.Vwma(input, volume, period: 1);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Vwma_WhenLatestWindowHasNoVolume_DoesNotCallSignalLogic()
    {
        double[] input = [40, 30, 20, 10];
        double[] volume = [0, 0, 3, 0];
        var invoked = false;

        var result = Indicators.Vwma(input, volume, 2, (source, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal([double.NaN, 20d, 20d, double.NaN], result.Ma);
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(2, result.Ma.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Vwma_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Vwma([2, 1], [2, 1], 3, (source, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Vwma_WithMinimumHistory_ReturnsOneValue()
    {
        var result = Indicators.Vwma([30, 20, 10], [3, 2, 1], period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(140d / 6, result.Ma[0], precision: 10);
        Assert.Equal(1, result.Ma.CountValidData());
        Assert.All(result.Ma[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void Vwma_DefaultSignalComparesLatestPriceAndOutput(bool rising, IndicatorSignal expected)
    {
        double[] input = rising ? [4, 3, 2, 1] : [1, 2, 3, 4];
        double[] volume = [1, 1, 1, 1];

        var result = Indicators.Vwma(input, volume, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Vwma_PassesNewestFirstPriceAndOutputToCustomSignalLogic()
    {
        double[] input = [50, 40, 30, 20, 10];
        double[] volume = [5, 4, 3, 2, 1];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;

        var result = Indicators.Vwma(input, volume, 3, (source, output) =>
        {
            callbackInput = (double[])source.Clone();
            callbackOutput = (double[])output.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackInput);
        Assert.NotNull(callbackOutput);
        Assert.Equal(input, callbackInput);
        Assert.Equal(result.Ma, callbackOutput);
        Assert.Equal(500d / 12, callbackOutput[0], precision: 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Vwma_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.Vwma([3, 2, 1], [3, 2, 1], period);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void Vwma_WhenSeriesLengthsDiffer_ReturnsFailure(int volumeLength)
    {
        var result = Indicators.Vwma(new double[5], new double[volumeLength], period: 3);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void Vwma_WhenInputIsNull_ReturnsFailure()
    {
        var missingInput = Indicators.Vwma(null!, [1, 1], period: 2);
        var missingVolume = Indicators.Vwma([2, 1], null!, period: 2);

        Assert.False(missingInput.Success);
        Assert.False(missingVolume.Success);
        Assert.Equal(IndicatorSignal.Stay, missingInput.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, missingVolume.IndicatorSignal);
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Close, 9.75)]
    [InlineData(IndicatorCalculationBase.Open, 19.75)]
    [InlineData(IndicatorCalculationBase.High, 29.75)]
    [InlineData(IndicatorCalculationBase.Low, 4.75)]
    [InlineData(IndicatorCalculationBase.Volume, 2.5)]
    public void Vwma_FromCandles_UsesSelectedSeriesAndCandleVolume(
        IndicatorCalculationBase calculationBase, double expected)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 5, Volume = 3 },
            new() { Close = 9, Open = 19, High = 29, Low = 4, Volume = 1 }
        };

        var result = Indicators.Vwma(candles, 2, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected, result.Ma[0], precision: 10);
        Assert.True(double.IsNaN(result.Ma[1]));
    }

    [Fact]
    public void Vwma_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, Volume = 3 },
            new() { Close = 9, Open = 19, Volume = 1 }
        };
        double[]? callbackInput = null;

        var defaultResult = Indicators.Vwma(candles, 2);
        var customResult = Indicators.Vwma(candles, 2, IndicatorCalculationBase.Open, (input, output) =>
        {
            callbackInput = (double[])input.Clone();
            return IndicatorSignal.Buy;
        });

        Assert.True(defaultResult.Success, defaultResult.Message);
        Assert.Equal(9.75, defaultResult.Ma[0], precision: 10);
        Assert.True(customResult.Success, customResult.Message);
        Assert.NotNull(callbackInput);
        Assert.Equal([20d, 19d], callbackInput);
        Assert.Equal(19.75, customResult.Ma[0], precision: 10);
        Assert.Equal(IndicatorSignal.Buy, customResult.IndicatorSignal);
    }
}
