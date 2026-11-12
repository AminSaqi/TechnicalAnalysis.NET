using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class VarTests
{
    [Fact]
    public void Var_ReturnsPopulationVarianceNewestFirstWithoutChangingInput()
    {
        double[] input = [5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var result = Indicators.Var(input, 2);
        Assert.True(result.Success, result.Message);
        Assert.Equal([0.25d, 0.25d, 0.25d, 0.25d, double.NaN], result.Var);
        Assert.Equal(original, input);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Var_MatchesTaLib(int period)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Var(0, input.Length - 1, input.Reverse().ToArray(), period, 1, expected);
        var result = Indicators.Var(input, period);
        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Var.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Var.Take(range.Count));
        Assert.All(result.Var.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Var.CountValidData());
        if (period == 1) Assert.All(result.Var, value => Assert.Equal(0d, value));
    }

    [Fact]
    public void Var_InsufficientHistoryDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Var([2, 1], 3, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Var, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Var_FromCandlesUsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle> { new() { Open = 5, Close = 9 }, new() { Open = 3, Close = 8 }, new() { Open = 1, Close = 7 } };
        var result = Indicators.Var(candles, 2, IndicatorCalculationBase.Open, values =>
        {
            Assert.Equal([1d, 1d, double.NaN], values);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(0.25d, Indicators.Var(candles, 2).Var[0]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Var_InvalidPeriodReturnsFailure(int period)
    {
        var result = Indicators.Var([3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Var_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Var((double[])null!, 2).Success);
        Assert.False(Indicators.Var(Array.Empty<double>(), 2).Success);
    }
}
