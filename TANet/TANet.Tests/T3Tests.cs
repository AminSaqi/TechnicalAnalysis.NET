using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class T3Tests
{
    [Theory]
    [InlineData(0d)]
    [InlineData(0.7d)]
    [InlineData(1d)]
    public void T3_MatchesOfficialTaLibAndPreservesInput(double vFactor)
    {
        double[] input =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10
        ];
        var originalInput = (double[])input.Clone();
        const int period = 3;

        var result = Indicators.T3(input, period, vFactor);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.T3(
            0, input.Length - 1, input.Reverse().ToArray(), period, vFactor, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(range.Count, result.Ma.CountValidData());
        Assert.Equal(originalInput, input);
    }

    [Fact]
    public void T3_WhenPeriodIsOne_ReturnsInputAndStay()
    {
        double[] input = [5, 4, 3, 2, 1];

        var result = Indicators.T3(input, period: 1, vFactor: 0.7);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void T3_WhenFactorChanges_ChangesComputedValues()
    {
        double[] input = [30, 27, 28, 24, 25, 23, 22, 24, 21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10];

        var first = Indicators.T3(input, period: 3, vFactor: 0);
        var second = Indicators.T3(input, period: 3, vFactor: 1);

        Assert.True(first.Success, first.Message);
        Assert.True(second.Success, second.Message);
        Assert.True(first.Ma.HasValidData());
        Assert.True(second.Ma.HasValidData());
        Assert.NotEqual(first.Ma[0], second.Ma[0]);
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void T3_DefaultSignalComparesLatestInputAndOutput(bool rising, IndicatorSignal expected)
    {
        var input = Enumerable.Range(1, 20).Select(value => (double)value).ToArray();
        if (rising)
        {
            Array.Reverse(input);
        }

        var result = Indicators.T3(input, period: 3, vFactor: 0);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void T3_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var input = Enumerable.Range(1, 12).Select(value => (double)value).ToArray();

        var result = Indicators.T3(input, 3, 0.7, (source, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(0, result.Ma.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void T3_WithMinimumHistory_ReturnsOneValue()
    {
        var input = Enumerable.Range(1, 13).Select(value => (double)value).ToArray();

        var result = Indicators.T3(input, period: 3, vFactor: 0.7);

        Assert.True(result.Success, result.Message);
        Assert.Equal(1, result.Ma.CountValidData());
        Assert.True(result.Ma.HasValidData());
        Assert.All(result.Ma[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void T3_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[] input = [30, 27, 28, 24, 25, 23, 22, 24, 21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;

        var result = Indicators.T3(input, 3, 0.7, (source, output) =>
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
        Assert.True(double.IsFinite(callbackOutput[0]));
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void T3_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        var input = Enumerable.Range(1, 20).Select(value => (double)value).ToArray();
        input[0] = double.NaN;
        var invoked = false;

        var result = Indicators.T3(input, 3, 0.7, (source, output) =>
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
    [InlineData(0, 0.7)]
    [InlineData(100001, 0.7)]
    [InlineData(3, -0.01)]
    [InlineData(3, 1.01)]
    public void T3_WhenParametersAreInvalid_ReturnsFailure(int period, double vFactor)
    {
        var result = Indicators.T3([5, 4, 3, 2, 1], period, vFactor);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Close, 10d)]
    [InlineData(IndicatorCalculationBase.Open, 20d)]
    [InlineData(IndicatorCalculationBase.High, 30d)]
    [InlineData(IndicatorCalculationBase.Low, 40d)]
    [InlineData(IndicatorCalculationBase.Volume, 50d)]
    public void T3_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double latest)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 40, Volume = 50 },
            new() { Close = 9, Open = 19, High = 29, Low = 39, Volume = 49 }
        };

        var result = Indicators.T3(candles, 1, 0.7, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal([latest, latest - 1], result.Ma);
    }

    [Fact]
    public void T3_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20 },
            new() { Close = 9, Open = 19 }
        };
        double[]? callbackInput = null;

        var defaultResult = Indicators.T3(candles, 1, 0.7);
        var customResult = Indicators.T3(candles, 1, 0.7, IndicatorCalculationBase.Open, (input, output) =>
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
