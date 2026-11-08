using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class HmaTests
{
    [Fact]
    public void Hma_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] input = [7, 6, 5, 4, 3, 2, 1];
        var original = (double[])input.Clone();

        var result = Indicators.Hma(input, period: 4);

        Assert.True(result.Success, result.Message);
        // For chronological prices 1..7, HMA(4) first produces 5, then 6 and 7.
        Assert.Equal([7d, 6d, 5d, double.NaN, double.NaN, double.NaN, double.NaN], result.Ma);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(3, result.Ma.CountValidData());
        Assert.Equal(original, input);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(7)]
    [InlineData(20)]
    public void Hma_MatchesOfficialTaLib(int period)
    {
        double[] input =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10, 9
        ];

        var result = Indicators.Hma(input, period);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Hma(0, input.Length - 1, input.Reverse().ToArray(), period, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(range.Count, result.Ma.CountValidData());
    }

    [Fact]
    public void Hma_WhenPeriodIsOne_ReturnsInputAndStay()
    {
        double[] input = [5, 4, 3, 2, 1];

        var result = Indicators.Hma(input, period: 1);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Sell)]
    [InlineData(false, IndicatorSignal.Buy)]
    public void Hma_DefaultSignalComparesLatestInputAndOutput(bool rising, IndicatorSignal expected)
    {
        double[] input = rising ? [4, 3, 2, 1] : [1, 2, 3, 4];

        var result = Indicators.Hma(input, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Hma_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Hma([4, 3, 2, 1], 4, (input, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(4, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(0, result.Ma.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Hma_WithMinimumHistory_ReturnsOneValue()
    {
        var result = Indicators.Hma([5, 4, 3, 2, 1], period: 4);

        Assert.True(result.Success, result.Message);
        Assert.Equal(5d, result.Ma[0], precision: 10);
        Assert.Equal(1, result.Ma.CountValidData());
        Assert.All(result.Ma[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Hma_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[] input = [7, 6, 5, 4, 3, 2, 1];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;

        var result = Indicators.Hma(input, 4, (source, output) =>
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
        Assert.Equal(7d, callbackOutput[0], precision: 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Hma_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        double[] input = [double.NaN, 6, 5, 4, 3, 2, 1];
        var invoked = false;

        var result = Indicators.Hma(input, 4, (source, output) =>
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
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Hma_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.Hma([5, 4, 3, 2, 1], period);

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
    public void Hma_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double latest)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 5, Volume = 50 },
            new() { Close = 9, Open = 19, High = 29, Low = 4, Volume = 49 }
        };

        var result = Indicators.Hma(candles, 1, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal([latest, latest - 1], result.Ma);
    }

    [Fact]
    public void Hma_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20 },
            new() { Close = 9, Open = 19 }
        };
        double[]? callbackInput = null;

        var defaultResult = Indicators.Hma(candles, 1);
        var customResult = Indicators.Hma(candles, 1, IndicatorCalculationBase.Open, (input, output) =>
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
