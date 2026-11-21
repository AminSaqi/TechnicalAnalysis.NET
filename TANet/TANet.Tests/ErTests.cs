using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class ErTests
{
    [Fact]
    public void Er_ReturnsKnownValuesAndDoesNotChangeInput()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var result = Indicators.Er(input, 2);
        Assert.True(result.Success, result.Message);
        Assert.Equal(1d, result.Er[0], 10);
        Assert.Equal(1d, result.Er[1], 10);
        Assert.Equal(1d, result.Er[2], 10);
        Assert.All(result.Er[3..], value => Assert.True(double.IsNaN(value)));
        Assert.Equal(original, input);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Er_MatchesTaLib(int period)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Er(0, input.Length - 1, input.Reverse().ToArray(), period, expected);
        var result = Indicators.Er(input, period);
        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Er.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Er.Take(range.Count));
        Assert.All(result.Er.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Er.CountValidData());
    }

    [Fact]
    public void Er_InsufficientHistoryDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Er([2, 1], 2, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Er, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Er_FromCandlesUsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle> { new() { Open = 5, Close = 9 }, new() { Open = 3, Close = 8 }, new() { Open = 1, Close = 7 } };
        var called = false;
        var result = Indicators.Er(candles, 2, IndicatorCalculationBase.Open, values =>
        {
            called = true;
            Assert.Equal(1d, values[0], 10);
            Assert.True(double.IsNaN(values[1]));
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(1d, Indicators.Er(candles, 2).Er[0], 10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    [InlineData(1)]
    public void Er_InvalidPeriodReturnsFailure(int period)
    {
        var result = Indicators.Er([3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Er_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Er((double[])null!, 2).Success);
        Assert.False(Indicators.Er(Array.Empty<double>(), 2).Success);
    }

}
