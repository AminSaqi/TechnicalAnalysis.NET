using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class TrangeTests
{
    [Fact]
    public void Trange_ReturnsKnownValuesNewestFirstWithoutChangingInputs()
    {
        double[] high = [16, 16, 14, 13, 11];
        double[] low = [13, 10, 10, 10, 9];
        double[] close = [14, 15, 11, 12, 10];
        var originals = new[] { high, low, close }.Select(x => (double[])x.Clone()).ToArray();
        var result = Indicators.Trange(high, low, close);
        Assert.True(result.Success, result.Message);
        Assert.Equal([3d, 6d, 4d, 3d, double.NaN], result.Trange);
        Assert.Equal(4, result.Trange.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originals[0], high);
        Assert.Equal(originals[1], low);
        Assert.Equal(originals[2], close);
    }

    [Fact]
    public void Trange_MatchesTaLib()
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        double[] close = [18, 17, 14, 13, 15, 12, 11];
        var expected = new double[high.Length];
        var range = TALib.Core.Default.Trange(0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(), expected);
        var result = Indicators.Trange(high, low, close);
        Assert.True(result.Success, result.Message);
        Assert.Equal(high.Length, result.Trange.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Trange.Take(range.Count));
        Assert.All(result.Trange.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Trange_OneBarHasNoPreviousCloseAndDoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Trange([11], [9], [10], _ => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.Equal([double.NaN], result.Trange);
        Assert.False(result.Trange.HasValidData());
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Trange_FromCandlesForwardsOutputToSignal()
    {
        var candles = new List<Candle> { new() { High = 13, Low = 11, Close = 12 }, new() { High = 11, Low = 9, Close = 10 } };
        var result = Indicators.Trange(candles, values =>
        {
            Assert.Equal([3d, double.NaN], values);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Trange(candles).IndicatorSignal);
    }

    [Fact]
    public void Trange_InvalidInputsReturnFailure()
    {
        var results = new[]
        {
            Indicators.Trange(null!, [1], [1]), Indicators.Trange([1], null!, [1]),
            Indicators.Trange([1], [1], null!), Indicators.Trange([], [], []),
            Indicators.Trange([2, 1], [1], [1]), Indicators.Trange([1], [1, 2], [1]),
            Indicators.Trange([1], [1], [1, 2])
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }
}
