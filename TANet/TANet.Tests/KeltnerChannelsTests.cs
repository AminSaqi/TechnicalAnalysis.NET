using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class KeltnerChannelsTests
{
    [Fact]
    public void KeltnerChannels_ReturnsKnownValuesAndDoesNotMutateInputs()
    {
        double[] high = [11, 11, 11, 11, 11];
        double[] low = [9, 9, 9, 9, 9];
        double[] close = [10, 10, 10, 10, 10];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.KeltnerChannels(high, low, close, 2, 2, 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([14d, 14d, 14d, double.NaN, double.NaN], result.UpperBand);
        Assert.Equal([10d, 10d, 10d, double.NaN, double.NaN], result.MiddleBand);
        Assert.Equal([6d, 6d, 6d, double.NaN, double.NaN], result.LowerBand);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
    }

    [Theory]
    [InlineData(2, 1, 2)]
    [InlineData(3, 2, 1.5)]
    [InlineData(2, 4, 0)]
    [InlineData(6, 2, 3)]
    public void KeltnerChannels_MatchesOfficialTaLibInCommonRange(int period, int atrPeriod, double multiplier)
    {
        double[] high = [18, 16, 17, 13, 14, 12, 11, 9];
        double[] low = [13, 10, 12, 8, 9, 6, 7, 4];
        double[] close = [16, 12, 15, 10, 11, 9, 8, 7];
        var upper = new double[high.Length];
        var middle = new double[high.Length];
        var lower = new double[high.Length];
        var range = TALib.Core.Default.Kc(0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            period, atrPeriod, multiplier, upper, middle, lower);

        var result = Indicators.KeltnerChannels(high, low, close, period, atrPeriod, multiplier);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        void AssertBand(double[] expected, double[] actual)
        {
            Assert.Equal(high.Length, actual.Length);
            Assert.Equal(expected.Take(range.Count).Reverse(), actual.Take(range.Count));
            Assert.True(actual.HasValidData());
            Assert.Equal(range.Count, actual.CountValidData());
            Assert.All(actual.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        }
        AssertBand(upper, result.UpperBand);
        AssertBand(middle, result.MiddleBand);
        AssertBand(lower, result.LowerBand);
    }

    [Fact]
    public void KeltnerChannels_WhenOnlyEmaIsReady_ReturnsNaNsWithoutCallingSignal()
    {
        var called = false;
        var result = Indicators.KeltnerChannels([15, 14, 13, 12], [11, 10, 9, 8], [13, 12, 11, 10],
            2, 6, 2, (_, _, _, _, _, _) => { called = true; return IndicatorSignal.Buy; });

        Assert.True(result.Success, result.Message);
        Assert.All(result.MiddleBand, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.UpperBand, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.LowerBand, value => Assert.True(double.IsNaN(value)));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void KeltnerChannels_MiddleBandMatchesKcWhenSeparateEmaDiffers()
    {
        double[] high = [18, 16, 17, 13, 14, 12, 11, 9];
        double[] low = [13, 10, 12, 8, 9, 6, 7, 4];
        double[] close = [16, 12, 15, 10, 11, 9, 8, 7];
        var typicalPrice = new double[high.Length];
        TALib.Core.Default.Typprice(0, high.Length - 1, high.Reverse().ToArray(),
            low.Reverse().ToArray(), close.Reverse().ToArray(), typicalPrice);
        var ema = new double[high.Length];
        var emaRange = TALib.Core.Default.Ema(0, high.Length - 1, typicalPrice, 2, ema);

        var kcUpper = new double[high.Length];
        var kcMiddle = new double[high.Length];
        var kcLower = new double[high.Length];
        var kcRange = TALib.Core.Default.Kc(0, high.Length - 1, high.Reverse().ToArray(),
            low.Reverse().ToArray(), close.Reverse().ToArray(), 2, 4, 2, kcUpper, kcMiddle, kcLower);
        var result = Indicators.KeltnerChannels(high, low, close, 2, 4, 2);

        Assert.True(result.Success, result.Message);
        Assert.NotEqual(ema[emaRange.Count - 1], kcMiddle[kcRange.Count - 1]);
        Assert.Equal(kcMiddle.Take(kcRange.Count).Reverse(), result.MiddleBand.Take(kcRange.Count));
        Assert.All(result.MiddleBand.Skip(kcRange.Count), value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void KeltnerChannels_DefaultPeriodSentinelUsesKcDefault()
    {
        var high = Enumerable.Range(1, 25).Select(i => (double)i + 10).Reverse().ToArray();
        var low = high.Select(value => value - 4).ToArray();
        var close = high.Select(value => value - 2).ToArray();

        var defaults = Indicators.KeltnerChannels(high, low, close, int.MinValue, 1, 2);
        var explicitPeriod = Indicators.KeltnerChannels(high, low, close, 20, 1, 2);

        Assert.True(defaults.Success, defaults.Message);
        Assert.Equal(explicitPeriod.MiddleBand, defaults.MiddleBand);
        Assert.Equal(6, defaults.MiddleBand.CountValidData());
        Assert.Equal(explicitPeriod.UpperBand, defaults.UpperBand);
        Assert.Equal(explicitPeriod.LowerBand, defaults.LowerBand);
    }

    [Fact]
    public void KeltnerChannels_WhenAllHistoryIsInsufficient_ReturnsNaNs()
    {
        var result = Indicators.KeltnerChannels([5], [3], [4], 3, 2, 2);
        Assert.True(result.Success, result.Message);
        foreach (var band in new[] { result.UpperBand, result.MiddleBand, result.LowerBand })
        {
            Assert.Equal([double.NaN], band);
            Assert.False(band.HasValidData());
        }
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void KeltnerChannels_CustomSignalReceivesInputsAndBandsInNewestFirstOrder()
    {
        double[] high = [13, 12, 11, 10];
        double[] low = [9, 8, 7, 6];
        double[] close = [11, 10, 9, 8];
        double[][]? received = null;
        var result = Indicators.KeltnerChannels(high, low, close, 2, 2, 2, (h, l, c, upper, middle, lower) =>
        {
            received = [h, l, c, upper, middle, lower];
            return IndicatorSignal.Sell;
        });
        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(high, received[0]);
        Assert.Equal(low, received[1]);
        Assert.Equal(close, received[2]);
        Assert.Equal(result.UpperBand, received[3]);
        Assert.Equal(result.MiddleBand, received[4]);
        Assert.Equal(result.LowerBand, received[5]);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void KeltnerChannels_WhenLatestBandIsNotFinite_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.KeltnerChannels([double.PositiveInfinity, 12, 11, 10], [8, 7, 6, 5],
            [10, 9, 8, 7], 2, 1, 2, (_, _, _, _, _, _) => { called = true; return IndicatorSignal.Buy; });
        Assert.True(result.Success, result.Message);
        Assert.False(result.UpperBand.HasValidData());
        Assert.True(double.IsFinite(result.UpperBand[1]));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 0)]
    [InlineData(100001, 2)]
    [InlineData(2, -1)]
    public void KeltnerChannels_WhenPeriodIsInvalid_ReturnsFailure(int period, int atrPeriod)
    {
        var result = Indicators.KeltnerChannels([5, 4, 3], [3, 2, 1], [4, 3, 2], period, atrPeriod, 2);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void KeltnerChannels_WhenInputsAreInvalid_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.KeltnerChannels(null!, [2, 1], [2, 1], 2, 1, 2),
            Indicators.KeltnerChannels([3, 2], null!, [2, 1], 2, 1, 2),
            Indicators.KeltnerChannels([3, 2], [2, 1], null!, 2, 1, 2),
            Indicators.KeltnerChannels([], [], [], 2, 1, 2),
            Indicators.KeltnerChannels([3, 2, 1], [2, 1], [2, 1], 2, 1, 2),
            Indicators.KeltnerChannels([3, 2], [2, 1], [3, 2, 1], 2, 1, 2)
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }

    [Fact]
    public void KeltnerChannels_FromCandles_UsesOhlcAndForwardsCallback()
    {
        var candles = new List<Candle>
        {
            new() { High = 13, Low = 9, Close = 11 },
            new() { High = 12, Low = 8, Close = 10 },
            new() { High = 11, Low = 7, Close = 9 }
        };
        var result = Indicators.KeltnerChannels(candles, 2, 1, 2, (high, low, close, _, _, _) =>
        {
            Assert.Equal([13d, 12d, 11d], high);
            Assert.Equal([9d, 8d, 7d], low);
            Assert.Equal([11d, 10d, 9d], close);
            return IndicatorSignal.Buy;
        });
        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.KeltnerChannels(candles, 2, 1, 2).IndicatorSignal);
    }
}
