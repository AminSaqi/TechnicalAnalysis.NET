using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class ZlemaTests
{
    [Fact]
    public void Zlema_WithOddPeriod_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] input = [32, 16, 8, 4, 2, 1];
        var original = (double[])input.Clone();

        var result = Indicators.Zlema(input, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Ma.Length);
        // Chronological de-lagged values are 3, 6, 12, 24, 48; their EMA(3) starts at 7.
        Assert.Equal([31.75, 15.5, 7d, double.NaN, double.NaN, double.NaN], result.Ma);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(3, result.Ma.CountValidData());
        Assert.Equal(original, input);
    }

    [Fact]
    public void Zlema_WithEvenPeriod_UsesTruncatedLag()
    {
        double[] input = [32, 16, 8, 4, 2, 1];

        var result = Indicators.Zlema(input, period: 4);

        Assert.True(result.Success, result.Message);
        // lag = floor((4 - 1) / 2) = 1; EMA(4) of the de-lagged series starts at 11.25.
        Assert.Equal(25.95, result.Ma[0], precision: 10);
        Assert.Equal(11.25, result.Ma[1], precision: 10);
        Assert.All(result.Ma[2..], value => Assert.True(double.IsNaN(value)));
        Assert.Equal(2, result.Ma.CountValidData());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void Zlema_MatchesOfficialTaLib(int period)
    {
        double[] input =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10, 9
        ];

        var result = Indicators.Zlema(input, period);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Zlema(0, input.Length - 1, input.Reverse().ToArray(), period, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(range.Count, result.Ma.CountValidData());
    }

    [Fact]
    public void Zlema_WhenPeriodIsOne_ReturnsInputAndStay()
    {
        double[] input = [5, 4, 3, 2, 1];

        var result = Indicators.Zlema(input, period: 1);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void Zlema_DefaultSignalComparesLatestInputAndOutput(bool rising, IndicatorSignal expected)
    {
        double[] input = rising ? [32, 16, 8, 4, 2, 1] : [-32, -16, -8, -4, -2, -1];

        var result = Indicators.Zlema(input, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Zlema_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Zlema([4, 2, 1], 3, (input, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(3, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(0, result.Ma.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Zlema_WithMinimumHistory_ReturnsOneValue()
    {
        var result = Indicators.Zlema([8, 4, 2, 1], period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(7d, result.Ma[0], precision: 10);
        Assert.Equal(1, result.Ma.CountValidData());
        Assert.All(result.Ma[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Zlema_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[] input = [32, 16, 8, 4, 2, 1];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;

        var result = Indicators.Zlema(input, 3, (source, output) =>
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
        Assert.Equal(31.75, callbackOutput[0], precision: 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Zlema_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        double[] input = [double.NaN, 16, 8, 4, 2, 1];
        var invoked = false;

        var result = Indicators.Zlema(input, 3, (source, output) =>
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
    public void Zlema_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.Zlema([5, 4, 3, 2, 1], period);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Zlema_WhenInputIsNull_ReturnsFailure()
    {
        var result = Indicators.Zlema((double[])null!, period: 3);

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
    public void Zlema_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double latest)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 5, Volume = 50 },
            new() { Close = 9, Open = 19, High = 29, Low = 4, Volume = 49 }
        };

        var result = Indicators.Zlema(candles, 1, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal([latest, latest - 1], result.Ma);
    }

    [Fact]
    public void Zlema_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20 },
            new() { Close = 9, Open = 19 }
        };
        double[]? callbackInput = null;

        var defaultResult = Indicators.Zlema(candles, 1);
        var customResult = Indicators.Zlema(candles, 1, IndicatorCalculationBase.Open, (input, output) =>
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
