using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class VidyaTests
{
    [Fact]
    public void Vidya_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();

        var result = Indicators.Vidya(input, period: 3, cmoPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Ma.Length);
        // For the rising chronological input, |CMO| is 100 and alpha is 1/2.
        Assert.Equal([4.0625, 3.125, 2.25, double.NaN, double.NaN], result.Ma);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(3, result.Ma.CountValidData());
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(original, input);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(3, 2)]
    [InlineData(5, 3)]
    [InlineData(12, 9)]
    [InlineData(2, 8)]
    public void Vidya_MatchesOfficialTaLib(int period, int cmoPeriod)
    {
        double[] input =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10, 9
        ];

        var result = Indicators.Vidya(input, period, cmoPeriod);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Vidya(
            0, input.Length - 1, input.Reverse().ToArray(), period, cmoPeriod, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(range.Count, result.Ma.CountValidData());
    }

    [Fact]
    public void Vidya_WhenCmoPeriodChanges_ChangesComputedValues()
    {
        double[] input = [15, 10, 14, 8, 13, 7, 12, 9, 11, 10, 8, 7, 9, 5, 6, 4, 3, 4, 2, 1];

        var first = Indicators.Vidya(input, period: 5, cmoPeriod: 2);
        var second = Indicators.Vidya(input, period: 5, cmoPeriod: 7);

        Assert.True(first.Success, first.Message);
        Assert.True(second.Success, second.Message);
        Assert.True(first.Ma.HasValidData());
        Assert.True(second.Ma.HasValidData());
        Assert.NotEqual(first.Ma[0], second.Ma[0]);
    }

    [Fact]
    public void Vidya_WhenPeriodIsOne_ReturnsInputAndStay()
    {
        double[] input = [5, 4, 3, 2, 1];

        var result = Indicators.Vidya(input, period: 1, cmoPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Vidya_WhenInputIsConstant_ReturnsConstantValuesAndStay()
    {
        double[] input = [10, 10, 10, 10, 10];

        var result = Indicators.Vidya(input, period: 3, cmoPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([10d, 10d, 10d, double.NaN, double.NaN], result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void Vidya_DefaultSignalComparesLatestInputAndOutput(bool rising, IndicatorSignal expected)
    {
        double[] input = rising ? [5, 4, 3, 2, 1] : [1, 2, 3, 4, 5];

        var result = Indicators.Vidya(input, period: 3, cmoPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Vidya_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Vidya([2, 1], 3, 2, (input, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(0, result.Ma.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Vidya_WithMinimumHistory_ReturnsOneValue()
    {
        var result = Indicators.Vidya([3, 2, 1], period: 3, cmoPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(2.25, result.Ma[0], precision: 10);
        Assert.Equal(1, result.Ma.CountValidData());
        Assert.All(result.Ma[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Vidya_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[] input = [5, 4, 3, 2, 1];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;

        var result = Indicators.Vidya(input, 3, 2, (source, output) =>
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
        Assert.Equal(4.0625, callbackOutput[0], precision: 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Vidya_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        double[] input = [double.NaN, 4, 3, 2, 1];
        var invoked = false;

        var result = Indicators.Vidya(input, 3, 2, (source, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.False(result.Ma.HasValidData());
        Assert.True(result.Ma.CountValidData() > 0);
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(-1, 2)]
    [InlineData(100001, 2)]
    [InlineData(3, 0)]
    [InlineData(3, 1)]
    [InlineData(3, 100001)]
    public void Vidya_WhenPeriodsAreInvalid_ReturnsFailure(int period, int cmoPeriod)
    {
        var result = Indicators.Vidya([5, 4, 3, 2, 1], period, cmoPeriod);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Vidya_WhenInputIsNull_ReturnsFailure()
    {
        var result = Indicators.Vidya((double[])null!, period: 3, cmoPeriod: 2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Close, 10d)]
    [InlineData(IndicatorCalculationBase.Open, 20d)]
    [InlineData(IndicatorCalculationBase.High, 30d)]
    [InlineData(IndicatorCalculationBase.Low, 5d)]
    [InlineData(IndicatorCalculationBase.Volume, 50d)]
    public void Vidya_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double latest)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 5, Volume = 50 },
            new() { Close = 9, Open = 19, High = 29, Low = 4, Volume = 49 }
        };

        var result = Indicators.Vidya(candles, 1, 2, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal([latest, latest - 1], result.Ma);
    }

    [Fact]
    public void Vidya_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20 },
            new() { Close = 9, Open = 19 }
        };
        double[]? callbackInput = null;

        var defaultResult = Indicators.Vidya(candles, 1, 2);
        var customResult = Indicators.Vidya(candles, 1, 2, IndicatorCalculationBase.Open, (input, output) =>
        {
            callbackInput = (double[])input.Clone();
            return IndicatorSignal.Buy;
        });

        Assert.True(defaultResult.Success, defaultResult.Message);
        Assert.Equal([10d, 9d], defaultResult.Ma);
        Assert.True(customResult.Success, customResult.Message);
        Assert.NotNull(callbackInput);
        Assert.Equal([20d, 19d], callbackInput);
        Assert.Equal(IndicatorSignal.Buy, customResult.IndicatorSignal);
    }
}
