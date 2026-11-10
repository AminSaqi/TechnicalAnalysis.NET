using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class IchimokuTests
{
    [Fact]
    public void Ichimoku_ReturnsEachLineAsSoonAsItsOwnHistoryIsAvailable()
    {
        double[] high = [13, 12, 11, 10, 9, 8];
        double[] low = [8, 7, 6, 5, 4, 3];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();

        var result = Indicators.Ichimoku(high, low, 2, 3, 5);

        Assert.True(result.Success, result.Message);
        Assert.Equal([10d, 9d, 8d, 7d, 6d, double.NaN], result.TenkanSen);
        Assert.Equal([9.5d, 8.5d, 7.5d, 6.5d, double.NaN, double.NaN], result.KijunSen);
        Assert.Equal([9.75d, 8.75d, 7.75d, 6.75d, double.NaN, double.NaN], result.SenkouSpanA);
        Assert.Equal([8.5d, 7.5d, double.NaN, double.NaN, double.NaN, double.NaN], result.SenkouSpanB);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
    }

    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(5, 2, 3)]
    [InlineData(3, 5, 2)]
    [InlineData(2, 2, 2)]
    public void Ichimoku_ValuesInCommonRangeMatchOfficialTaLib(int tenkanPeriod, int kijunPeriod, int senkouBPeriod)
    {
        double[] high = [18, 16, 17, 13, 14, 12, 11, 9];
        double[] low = [13, 10, 12, 8, 9, 6, 7, 4];
        var expected = Enumerable.Range(0, 4).Select(_ => new double[high.Length]).ToArray();
        var range = TALib.Core.Default.Ichimoku(0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), tenkanPeriod, kijunPeriod, senkouBPeriod,
            expected[0], expected[1], expected[2], expected[3]);

        var result = Indicators.Ichimoku(high, low, tenkanPeriod, kijunPeriod, senkouBPeriod);

        Assert.True(result.Success, result.Message);
        double[][] actual = [result.TenkanSen, result.KijunSen, result.SenkouSpanA, result.SenkouSpanB];
        Assert.True(range.Count > 0);
        for (var i = 0; i < actual.Length; i++)
        {
            Assert.Equal(high.Length, actual[i].Length);
            Assert.Equal(expected[i].Take(range.Count).Reverse(), actual[i].Take(range.Count));
        }
        Assert.Equal(high.Length - tenkanPeriod + 1, result.TenkanSen.CountValidData());
        Assert.Equal(high.Length - kijunPeriod + 1, result.KijunSen.CountValidData());
        Assert.Equal(high.Length - Math.Max(tenkanPeriod, kijunPeriod) + 1, result.SenkouSpanA.CountValidData());
        Assert.Equal(high.Length - senkouBPeriod + 1, result.SenkouSpanB.CountValidData());
    }

    [Fact]
    public void Ichimoku_WithPartialHistory_ReturnsAvailableLinesWithoutCallingSignal()
    {
        var called = false;
        var result = Indicators.Ichimoku([5, 4], [2, 1], 2, 3, 5, (_, _, _, _, _, _) =>
        {
            called = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal([3d, double.NaN], result.TenkanSen);
        Assert.All(result.KijunSen, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.SenkouSpanA, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.SenkouSpanB, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Ichimoku_CustomSignalReceivesInputsAndUnshiftedLines()
    {
        double[] high = [13, 12, 11, 10, 9, 8];
        double[] low = [8, 7, 6, 5, 4, 3];
        double[][]? received = null;
        var result = Indicators.Ichimoku(high, low, 2, 3, 5, (h, l, tenkan, kijun, a, b) =>
        {
            received = [h, l, tenkan, kijun, a, b];
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(high, received[0]);
        Assert.Equal(low, received[1]);
        Assert.Equal(result.TenkanSen, received[2]);
        Assert.Equal(result.KijunSen, received[3]);
        Assert.Equal(result.SenkouSpanA, received[4]);
        Assert.Equal(result.SenkouSpanB, received[5]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Ichimoku_WhenLatestLineIsNotFinite_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Ichimoku([double.PositiveInfinity, 12, 11, 10], [8, 7, 6, 5], 2, 2, 2,
            (_, _, _, _, _, _) => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.False(result.TenkanSen.HasValidData());
        Assert.True(double.IsFinite(result.TenkanSen[1]));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Ichimoku_DefaultPeriodSentinelsMatchNineTwentySixFiftyTwo()
    {
        var high = Enumerable.Range(1, 60).Select(i => (double)i + 10).Reverse().ToArray();
        var low = high.Select(value => value - 5).ToArray();
        var defaults = Indicators.Ichimoku(high, low, int.MinValue, int.MinValue, int.MinValue);
        var explicitPeriods = Indicators.Ichimoku(high, low, 9, 26, 52);
        Assert.True(defaults.Success, defaults.Message);
        Assert.Equal(explicitPeriods.TenkanSen, defaults.TenkanSen);
        Assert.Equal(explicitPeriods.KijunSen, defaults.KijunSen);
        Assert.Equal(explicitPeriods.SenkouSpanA, defaults.SenkouSpanA);
        Assert.Equal(explicitPeriods.SenkouSpanB, defaults.SenkouSpanB);
    }

    [Theory]
    [InlineData(1, 2, 2)]
    [InlineData(2, 0, 2)]
    [InlineData(2, 2, 100001)]
    [InlineData(-1, 2, 2)]
    public void Ichimoku_InvalidPeriodsReturnFailure(int tenkan, int kijun, int spanB)
    {
        var result = Indicators.Ichimoku([4, 3, 2], [3, 2, 1], tenkan, kijun, spanB);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Ichimoku_InvalidInputReturnsFailure()
    {
        var results = new[]
        {
            Indicators.Ichimoku(null!, [1, 2], 2, 2, 2),
            Indicators.Ichimoku([1, 2], null!, 2, 2, 2),
            Indicators.Ichimoku([], [], 2, 2, 2),
            Indicators.Ichimoku([3, 2, 1], [2, 1], 2, 2, 2),
            Indicators.Ichimoku([2, 1], [3, 2, 1], 2, 2, 2)
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }

    [Fact]
    public void Ichimoku_FromCandlesUsesHighAndLowAndForwardsCallback()
    {
        var candles = new List<Candle>
        {
            new() { High = 13, Low = 8 }, new() { High = 12, Low = 7 }, new() { High = 11, Low = 6 }
        };
        var result = Indicators.Ichimoku(candles, 2, 2, 2, (high, low, _, _, _, _) =>
        {
            Assert.Equal([13d, 12d, 11d], high);
            Assert.Equal([8d, 7d, 6d], low);
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal([10d, 9d, double.NaN], result.TenkanSen);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Ichimoku(candles, 2, 2, 2).IndicatorSignal);
    }
}
