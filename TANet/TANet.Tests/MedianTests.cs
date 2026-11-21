using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class MedianTests
{
    [Fact]
    public void Median_ReturnsKnownEvenAndOddWindowsWithoutChangingInput()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var even = Indicators.Median(input, 2);
        var odd = Indicators.Median(input, 3);
        Assert.True(even.Success, even.Message);
        Assert.True(odd.Success, odd.Message);
        Assert.Equal([4.5d, 3.5d, 2.5d, 1.5d, double.NaN], even.Median);
        Assert.Equal([4d, 3d, 2d, double.NaN, double.NaN], odd.Median);
        Assert.Equal(original, input);
        Assert.Equal(IndicatorSignal.Stay, even.IndicatorSignal);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Median_MatchesTaLib(int period)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Median(0, input.Length - 1, input.Reverse().ToArray(), period, expected);
        var result = Indicators.Median(input, period);
        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Median.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Median.Take(range.Count));
        Assert.All(result.Median.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Median.CountValidData());
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Median_NonFiniteInputFailsWithoutCallingSignal(double invalid)
    {
        var called = false;
        var result = Indicators.Median([5, 4, invalid, 2, 1], 2, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("finite", result.Message);
        Assert.False(called);
    }

    [Fact]
    public void Median_InsufficientHistoryDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Median([2, 1], 3, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Median, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Median_FromCandlesUsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle> { new() { Open = 5, Close = 9 }, new() { Open = 3, Close = 8 }, new() { Open = 1, Close = 7 } };
        var result = Indicators.Median(candles, 2, IndicatorCalculationBase.Open, values =>
        {
            Assert.Equal([4d, 2d, double.NaN], values);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(8.5d, Indicators.Median(candles, 2).Median[0]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10001)]
    public void Median_InvalidPeriodReturnsFailure(int period)
    {
        var result = Indicators.Median([3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Median_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Median((double[])null!, 2).Success);
        Assert.False(Indicators.Median(Array.Empty<double>(), 2).Success);
    }
}
