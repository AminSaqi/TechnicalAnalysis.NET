using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class RocTests
{
    [Fact]
    public void Roc_ReturnsKnownValuesAndDoesNotChangeInput()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var result = Indicators.Roc(input, 2);
        Assert.True(result.Success, result.Message);
        Assert.Equal((5d / 3 - 1) * 100, result.Roc[0], 10);
        Assert.Equal(100d, result.Roc[1], 10);
        Assert.Equal(200d, result.Roc[2], 10);
        Assert.All(result.Roc[3..], value => Assert.True(double.IsNaN(value)));
        Assert.Equal(original, input);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Roc_MatchesTaLib(int period)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Roc(0, input.Length - 1, input.Reverse().ToArray(), period, expected);
        var result = Indicators.Roc(input, period);
        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Roc.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Roc.Take(range.Count));
        Assert.All(result.Roc.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Roc.CountValidData());
    }

    [Fact]
    public void Roc_InsufficientHistoryDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Roc([2, 1], 2, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Roc, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Roc_FromCandlesUsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle> { new() { Open = 5, Close = 9 }, new() { Open = 3, Close = 8 }, new() { Open = 1, Close = 7 } };
        var called = false;
        var result = Indicators.Roc(candles, 2, IndicatorCalculationBase.Open, values =>
        {
            called = true;
            Assert.Equal(400d, values[0], 10);
            Assert.True(double.IsNaN(values[1]));
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal((9d / 7 - 1) * 100, Indicators.Roc(candles, 2).Roc[0], 10);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Roc_InvalidPeriodReturnsFailure(int period)
    {
        var result = Indicators.Roc([3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Roc_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Roc((double[])null!, 2).Success);
        Assert.False(Indicators.Roc(Array.Empty<double>(), 2).Success);
    }

    [Fact]
    public void Roc_DefaultSignalReflectsDirection()
    {
        Assert.Equal(IndicatorSignal.Buy, Indicators.Roc([3, 2, 1], 2).IndicatorSignal);
        Assert.Equal(IndicatorSignal.Sell, Indicators.Roc([1, 2, 3], 2).IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Roc([1, 2, 1], 2).IndicatorSignal);
    }

    [Fact]
    public void Roc_ZeroDenominatorKeepsReferenceOutputAndDefaultStayButCallsCustomSignal()
    {
        var result = Indicators.Roc([3, 2, 0], 2);
        var called = false;
        var custom = Indicators.Roc([3, 2, 0], 2, values =>
        {
            called = true;
            Assert.Equal(0d, values[0]);
            return IndicatorSignal.Buy;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.Roc[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Buy, custom.IndicatorSignal);
    }
}
