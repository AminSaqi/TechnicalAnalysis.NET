using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class StddevTests
{
    [Fact]
    public void Stddev_ReturnsPopulationDeviationNewestFirstWithoutMutatingInput()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var result = Indicators.Stddev(input, 2, 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([1d, 1d, 1d, 1d, double.NaN], result.Stddev);
        Assert.Equal(original, input);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(0.5d, Indicators.Stddev(input, 2).Stddev[0]);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(5, 0)]
    public void Stddev_MatchesTaLib(int period, double multiplier)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Stddev(0, input.Length - 1, input.Reverse().ToArray(), period, multiplier, expected);
        var result = Indicators.Stddev(input, period, multiplier);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Stddev.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Stddev.Take(range.Count));
        Assert.All(result.Stddev.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Stddev.CountValidData());
    }

    [Fact]
    public void Stddev_WhenHistoryIsInsufficient_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Stddev([2, 1], 3, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Stddev, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Stddev_FromCandles_UsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle> { new() { Open = 5, Close = 9 }, new() { Open = 3, Close = 8 }, new() { Open = 1, Close = 7 } };
        var result = Indicators.Stddev(candles, 2, 2, IndicatorCalculationBase.Open, values =>
        {
            Assert.Equal([2d, 2d, double.NaN], values);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(0.5d, Indicators.Stddev(candles, 2).Stddev[0]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100001)]
    public void Stddev_InvalidPeriodReturnsFailure(int period)
    {
        var result = Indicators.Stddev([3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Stddev_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Stddev((double[])null!, 2).Success);
        Assert.False(Indicators.Stddev(Array.Empty<double>(), 2).Success);
    }
}
