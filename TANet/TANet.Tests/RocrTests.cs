using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class RocrTests
{
    [Fact]
    public void Rocr_ReturnsKnownValuesAndDoesNotChangeInput()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var result = Indicators.Rocr(input, 2);
        Assert.True(result.Success, result.Message);
        Assert.Equal(5d / 3, result.Rocr[0], 10);
        Assert.Equal(2d, result.Rocr[1], 10);
        Assert.Equal(3d, result.Rocr[2], 10);
        Assert.All(result.Rocr[3..], value => Assert.True(double.IsNaN(value)));
        Assert.Equal(original, input);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Rocr_MatchesTaLib(int period)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Rocr(0, input.Length - 1, input.Reverse().ToArray(), period, expected);
        var result = Indicators.Rocr(input, period);
        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Rocr.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Rocr.Take(range.Count));
        Assert.All(result.Rocr.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Rocr.CountValidData());
    }

    [Fact]
    public void Rocr_InsufficientHistoryDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Rocr([2, 1], 2, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Rocr, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Rocr_FromCandlesUsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle> { new() { Open = 5, Close = 9 }, new() { Open = 3, Close = 8 }, new() { Open = 1, Close = 7 } };
        var called = false;
        var result = Indicators.Rocr(candles, 2, IndicatorCalculationBase.Open, values =>
        {
            called = true;
            Assert.Equal(5d, values[0], 10);
            Assert.True(double.IsNaN(values[1]));
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(9d / 7, Indicators.Rocr(candles, 2).Rocr[0], 10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Rocr_InvalidPeriodReturnsFailure(int period)
    {
        var result = Indicators.Rocr([3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Rocr_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Rocr((double[])null!, 2).Success);
        Assert.False(Indicators.Rocr(Array.Empty<double>(), 2).Success);
    }

    [Fact]
    public void Rocr_DefaultSignalReflectsDirection()
    {
        Assert.Equal(IndicatorSignal.Buy, Indicators.Rocr([3, 2, 1], 2).IndicatorSignal);
        Assert.Equal(IndicatorSignal.Sell, Indicators.Rocr([1, 2, 3], 2).IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Rocr([1, 2, 1], 2).IndicatorSignal);
    }

    [Fact]
    public void Rocr_ZeroDenominatorKeepsReferenceOutputAndDefaultStayButCallsCustomSignal()
    {
        var result = Indicators.Rocr([3, 2, 0], 2);
        var called = false;
        var custom = Indicators.Rocr([3, 2, 0], 2, values =>
        {
            called = true;
            Assert.Equal(0d, values[0]);
            return IndicatorSignal.Buy;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.Rocr[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Buy, custom.IndicatorSignal);
    }

    [Fact]
    public void Rocr_DefaultPeriodSentinelChecksTheCorrectReferencePrice()
    {
        double[] input = [10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0];
        var result = Indicators.Rocr(input, int.MinValue);
        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.Rocr[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }
}
