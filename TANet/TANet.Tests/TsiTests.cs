using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class TsiTests
{
    [Fact]
    public void Tsi_RisingPricesYieldOneHundredWithoutChangingInput()
    {
        double[] input = [6, 5, 4, 3, 2, 1];
        var original = (double[])input.Clone();
        var result = Indicators.Tsi(input, 2, 2);
        Assert.True(result.Success, result.Message);
        Assert.Equal(100d, result.Tsi[0], 10);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(original, input);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    public void Tsi_MatchesTaLibWithPeriodsInDeclaredOrder(int firstPeriod, int secondPeriod)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4, 8, 2];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Tsi(0, input.Length - 1, input.Reverse().ToArray(), firstPeriod, secondPeriod, expected);
        var result = Indicators.Tsi(input, firstPeriod, secondPeriod);
        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Tsi.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Tsi.Take(range.Count));
        Assert.All(result.Tsi.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Tsi.CountValidData());
    }

    [Fact]
    public void Tsi_DefaultSignalReflectsCurrentDirection()
    {
        Assert.Equal(IndicatorSignal.Buy, Indicators.Tsi([6, 5, 4, 3, 2, 1], 2, 2).IndicatorSignal);
        Assert.Equal(IndicatorSignal.Sell, Indicators.Tsi([1, 2, 3, 4, 5, 6], 2, 2).IndicatorSignal);
        var flat = Indicators.Tsi([1, 1, 1, 1, 1, 1], 2, 2);
        Assert.True(flat.Success, flat.Message);
        Assert.Equal(0d, flat.Tsi[0]);
        Assert.Equal(IndicatorSignal.Stay, flat.IndicatorSignal);
    }

    [Fact]
    public void Tsi_InsufficientHistoryDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Tsi([2, 1], 2, 2, _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.All(result.Tsi, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Tsi_FromCandlesUsesSelectedPriceAndForwardsSignal()
    {
        var candles = new List<Candle>
        {
            new() { Open = 6, Close = 1 }, new() { Open = 5, Close = 2 },
            new() { Open = 4, Close = 3 }, new() { Open = 3, Close = 4 },
            new() { Open = 2, Close = 5 }, new() { Open = 1, Close = 6 }
        };
        var called = false;
        var result = Indicators.Tsi(candles, 2, 2, IndicatorCalculationBase.Open, values =>
        {
            called = true;
            Assert.Equal(100d, values[0], 10);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Sell, Indicators.Tsi(candles, 2, 2).IndicatorSignal);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(0, 2)]
    [InlineData(2, 100001)]
    public void Tsi_InvalidPeriodsReturnFailure(int firstPeriod, int secondPeriod)
    {
        var result = Indicators.Tsi([3, 2, 1], firstPeriod, secondPeriod);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Tsi_NullOrEmptyReturnsFailure()
    {
        Assert.False(Indicators.Tsi((double[])null!, 2, 2).Success);
        Assert.False(Indicators.Tsi(Array.Empty<double>(), 2, 2).Success);
    }
}
