using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class SupertrendTests
{
    [Fact]
    public void Supertrend_ReturnsKnownValuesAndDoesNotMutateInputs()
    {
        double[] high = [11, 11, 11, 11, 11];
        double[] low = [9, 9, 9, 9, 9];
        double[] close = [10, 10, 10, 10, 10];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.Supertrend(high, low, close, 2, 1);

        Assert.True(result.Success, result.Message);
        Assert.Equal([8d, 8d, 8d, double.NaN, double.NaN], result.Supertrend);
        Assert.Equal([1d, 1d, 1d, double.NaN, double.NaN], result.Trend);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
    }

    [Theory]
    [InlineData(2, 1)]
    [InlineData(3, 2.5)]
    [InlineData(2, 0)]
    [InlineData(5, 1.5)]
    public void Supertrend_MatchesOfficialTaLib(int period, double multiplier)
    {
        double[] close = [18, 17, 16, 11, 10, 9, 8, 9, 10, 14, 15, 16];
        var high = close.Select(value => value + 1).ToArray();
        var low = close.Select(value => value - 1).ToArray();
        var line = new double[close.Length];
        var trend = new int[close.Length];
        var range = TALib.Core.Default.Supertrend(0, close.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            period, multiplier, line, trend);

        var result = Indicators.Supertrend(high, low, close, period, multiplier);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(close.Length, result.Supertrend.Length);
        Assert.Equal(close.Length, result.Trend.Length);
        Assert.Equal(line.Take(range.Count).Reverse(), result.Supertrend.Take(range.Count));
        Assert.Equal(trend.Take(range.Count).Reverse().Select(value => (double)value), result.Trend.Take(range.Count));
        Assert.All(result.Supertrend.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.All(result.Trend.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.Supertrend.CountValidData());
        Assert.Equal(range.Count, result.Trend.CountValidData());
    }

    [Fact]
    public void Supertrend_WhenTrendChanges_ReportsDirectionsAndCurrentDirectionSignal()
    {
        double[] close = [18, 17, 16, 11, 10, 9, 8, 9, 10, 14, 15, 16];
        var result = Indicators.Supertrend(close.Select(x => x + 1).ToArray(),
            close.Select(x => x - 1).ToArray(), close, 2, 1);

        Assert.True(result.Success, result.Message);
        Assert.Contains(1d, result.Trend);
        Assert.Contains(-1d, result.Trend);
        Assert.Equal(1d, result.Trend[0]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Supertrend_WhenCurrentDirectionIsDown_ReturnsSell()
    {
        double[] close = [2, 3, 4, 5, 6, 7];
        var result = Indicators.Supertrend(close.Select(x => x + 1).ToArray(),
            close.Select(x => x - 1).ToArray(), close, 2, 1);

        Assert.True(result.Success, result.Message);
        Assert.Equal(-1d, result.Trend[0]);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Supertrend_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignal()
    {
        var called = false;
        var result = Indicators.Supertrend([5, 4], [3, 2], [4, 3], 2, 1,
            (_, _, _, _, _) => { called = true; return IndicatorSignal.Buy; });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Supertrend.Length);
        Assert.Equal(2, result.Trend.Length);
        Assert.All(result.Supertrend, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.Trend, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Supertrend.HasValidData());
        Assert.False(result.Trend.HasValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Fact]
    public void Supertrend_CustomSignalReceivesInputAndOutputSeries()
    {
        double[] high = [13, 12, 11, 10];
        double[] low = [9, 8, 7, 6];
        double[] close = [11, 10, 9, 8];
        double[][]? received = null;
        var result = Indicators.Supertrend(high, low, close, 2, 1, (h, l, c, line, trend) =>
        {
            received = [h, l, c, line, trend];
            return IndicatorSignal.Stay;
        });
        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(high, received[0]);
        Assert.Equal(low, received[1]);
        Assert.Equal(close, received[2]);
        Assert.Equal(result.Supertrend, received[3]);
        Assert.Equal(result.Trend, received[4]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Supertrend_WhenLatestLineIsNotFinite_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Supertrend([double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity],
            [8, 7, 6], [10, 9, 8], 2, 1,
            (_, _, _, _, _) => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.False(result.Supertrend.HasValidData());
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(100001, 1)]
    [InlineData(2, -1)]
    public void Supertrend_WhenParametersAreInvalid_ReturnsFailure(int period, double multiplier)
    {
        var result = Indicators.Supertrend([5, 4, 3], [3, 2, 1], [4, 3, 2], period, multiplier);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Supertrend_WhenInputsAreInvalid_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.Supertrend(null!, [2, 1], [2, 1], 2, 1),
            Indicators.Supertrend([3, 2], null!, [2, 1], 2, 1),
            Indicators.Supertrend([3, 2], [2, 1], null!, 2, 1),
            Indicators.Supertrend([], [], [], 2, 1),
            Indicators.Supertrend([3, 2, 1], [2, 1], [2, 1], 2, 1),
            Indicators.Supertrend([3, 2], [2, 1], [3, 2, 1], 2, 1)
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }

    [Fact]
    public void Supertrend_FromCandles_UsesHighLowCloseAndForwardsCallback()
    {
        var candles = new List<Candle>
        {
            new() { High = 13, Low = 9, Close = 11 },
            new() { High = 12, Low = 8, Close = 10 },
            new() { High = 11, Low = 7, Close = 9 }
        };
        var result = Indicators.Supertrend(candles, 2, 1, (high, low, close, _, _) =>
        {
            Assert.Equal([13d, 12d, 11d], high);
            Assert.Equal([9d, 8d, 7d], low);
            Assert.Equal([11d, 10d, 9d], close);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Buy, Indicators.Supertrend(candles, 2, 1).IndicatorSignal);
    }
}
