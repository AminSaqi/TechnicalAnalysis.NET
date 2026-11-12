using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class AdxTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Adx_StrongTrendsInEitherDirection_ReturnOneHundredAndStay(bool rising)
    {
        double[] close = rising ? [15, 14, 13, 12, 11, 10] : [10, 11, 12, 13, 14, 15];
        var high = close.Select(x => x + 1).ToArray();
        var low = close.Select(x => x - 1).ToArray();
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.Adx(high, low, close, 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([100d, 100d, 100d, double.NaN, double.NaN, double.NaN], result.Adx);
        Assert.True(result.Adx.HasValidData());
        Assert.Equal(3, result.Adx.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void Adx_MatchesOfficialTaLib(int period)
    {
        double[] high = [22, 20, 21, 17, 19, 16, 15, 14, 13, 11, 12, 10];
        double[] low = [18, 16, 15, 13, 14, 12, 11, 9, 9, 7, 8, 6];
        double[] close = [20, 19, 16, 15, 17, 14, 13, 12, 10, 9, 10, 8];
        var expected = new double[high.Length];
        var range = TALib.Core.Default.Adx(0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(), period, expected);

        var result = Indicators.Adx(high, low, close, period);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.Adx.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Adx.Take(range.Count));
        Assert.All(result.Adx.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Adx.CountValidData());
    }

    [Fact]
    public void Adx_FlatPrices_ReturnZeroAndStay()
    {
        var result = Indicators.Adx([11, 11, 11, 11], [9, 9, 9, 9], [10, 10, 10, 10], 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([0d, double.NaN, double.NaN, double.NaN], result.Adx);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Adx_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignal()
    {
        var called = false;
        var result = Indicators.Adx([13, 12, 11], [11, 10, 9], [12, 11, 10], 2, _ =>
        {
            called = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(3, result.Adx.Length);
        Assert.All(result.Adx, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Adx.HasValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Fact]
    public void Adx_WhenExactlyEnoughHistoryExists_ReturnsOneValueAndCallsSignal()
    {
        var called = false;
        var result = Indicators.Adx([14, 13, 12, 11], [12, 11, 10, 9], [13, 12, 11, 10], 2, values =>
        {
            called = true;
            Assert.Equal([100d, double.NaN, double.NaN, double.NaN], values);
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(1, result.Adx.CountValidData());
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Adx_CustomSignalReceivesTheCompleteNewestFirstSeries()
    {
        double[]? received = null;
        var result = Indicators.Adx([16, 15, 14, 13, 12, 11], [14, 13, 12, 11, 10, 9],
            [15, 14, 13, 12, 11, 10], 2, values =>
            {
                received = (double[])values.Clone();
                return IndicatorSignal.Sell;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(result.Adx, received);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Adx_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.Adx([4, 3, 2], [2, 1, 0], [3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Adx_WhenInputsAreInvalid_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.Adx(null!, [2, 1], [2, 1], 2),
            Indicators.Adx([3, 2], null!, [2, 1], 2),
            Indicators.Adx([3, 2], [2, 1], null!, 2),
            Indicators.Adx([], [], [], 2),
            Indicators.Adx([3, 2, 1], [2, 1], [2, 1], 2),
            Indicators.Adx([3, 2], [2, 1], [3, 2, 1], 2)
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }

    [Fact]
    public void Adx_FromCandles_UsesHighLowCloseAndForwardsCallback()
    {
        var candles = new List<Candle>
        {
            new() { High = 14, Low = 12, Close = 13 },
            new() { High = 13, Low = 11, Close = 12 },
            new() { High = 12, Low = 10, Close = 11 },
            new() { High = 11, Low = 9, Close = 10 }
        };
        var result = Indicators.Adx(candles, 2, output =>
        {
            Assert.Equal([100d, double.NaN, double.NaN, double.NaN], output);
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Adx(candles, 2).IndicatorSignal);
    }
}
