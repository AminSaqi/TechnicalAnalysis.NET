using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class VwapTests
{
    [Fact]
    public void Vwap_ReturnsKnownCumulativeValuesNewestFirstWithoutMutatingInputs()
    {
        double[] high = [31, 21, 11];
        double[] low = [29, 19, 9];
        double[] close = [30, 20, 10];
        double[] volume = [3, 2, 1];
        var originals = new[] { high, low, close, volume }.Select(x => (double[])x.Clone()).ToArray();

        var result = Indicators.Vwap(high, low, close, volume);

        Assert.True(result.Success, result.Message);
        Assert.Equal(140d / 6, result.Vwap[0], 10);
        Assert.Equal(50d / 3, result.Vwap[1], 10);
        Assert.Equal(10d, result.Vwap[2]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originals[0], high);
        Assert.Equal(originals[1], low);
        Assert.Equal(originals[2], close);
        Assert.Equal(originals[3], volume);
    }

    [Fact]
    public void Vwap_MatchesOfficialTaLib()
    {
        double[] high = [18, 16, 17, 13, 14, 12, 11, 9];
        double[] low = [13, 10, 12, 8, 9, 6, 7, 4];
        double[] close = [16, 12, 15, 10, 11, 9, 8, 7];
        double[] volume = [4, 6, 3, 0, 8, 4, 2, 5];
        var expected = new double[high.Length];
        var range = TALib.Core.Default.Vwap(0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            volume.Reverse().ToArray(), expected);

        var result = Indicators.Vwap(high, low, close, volume);

        Assert.True(result.Success, result.Message);
        Assert.Equal(high.Length, result.Vwap.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Vwap.Take(range.Count));
        Assert.Equal(high.Length, result.Vwap.CountValidData());
        Assert.True(result.Vwap.HasValidData());
    }

    [Fact]
    public void Vwap_ZeroVolumeCarriesPreviousValueAndStartsAtZero()
    {
        var result = Indicators.Vwap([41, 31, 21, 11], [39, 29, 19, 9], [40, 30, 20, 10], [0, 2, 0, 0]);

        Assert.True(result.Success, result.Message);
        Assert.Equal([30d, 30d, 0d, 0d], result.Vwap);
    }

    [Fact]
    public void Vwap_AllZeroVolumesReturnFiniteZerosAndAllowCustomSignal()
    {
        var called = false;
        var result = Indicators.Vwap([11, 10], [9, 8], [10, 9], [0, 0], (_, _, _, _, values) =>
        {
            called = true;
            Assert.Equal([0d, 0d], values);
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.True(result.Vwap.HasValidData());
        Assert.Equal(2, result.Vwap.CountValidData());
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Vwap([11, 10], [9, 8], [10, 9], [0, 0]).IndicatorSignal);
    }

    [Fact]
    public void Vwap_NonFiniteBarIsSkippedAndLaterValidBarsResume()
    {
        var result = Indicators.Vwap([41, 31, 21, 11], [39, 29, 19, 9],
            [40, double.NaN, 20, 10], [1, 1, 1, 1]);

        Assert.True(result.Success, result.Message);
        Assert.Equal(70d / 3, result.Vwap[0], 10);
        Assert.Equal([15d, 15d, 10d], result.Vwap[1..]);
    }

    [Fact]
    public void Vwap_AnchorIsOldestSuppliedBar()
    {
        var fullRange = Indicators.Vwap([31, 21, 11], [29, 19, 9], [30, 20, 10], [3, 2, 1]);
        var newerRange = Indicators.Vwap([31, 21], [29, 19], [30, 20], [3, 2]);

        Assert.True(fullRange.Success, fullRange.Message);
        Assert.True(newerRange.Success, newerRange.Message);
        Assert.Equal(140d / 6, fullRange.Vwap[0], 10);
        Assert.Equal(26d, newerRange.Vwap[0], 10);
    }

    [Fact]
    public void Vwap_CustomSignalReceivesInputsAndOutputNewestFirst()
    {
        double[] high = [31, 21, 11];
        double[] low = [29, 19, 9];
        double[] close = [30, 20, 10];
        double[] volume = [3, 2, 1];
        double[][]? received = null;
        var result = Indicators.Vwap(high, low, close, volume, (h, l, c, v, output) =>
        {
            received = [h, l, c, v, output];
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(high, received[0]);
        Assert.Equal(low, received[1]);
        Assert.Equal(close, received[2]);
        Assert.Equal(volume, received[3]);
        Assert.Equal(result.Vwap, received[4]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Vwap_FromCandlesUsesOhlcvAndDoesNotResetAtDateBoundary()
    {
        var candles = new List<Candle>
        {
            new() { High = 31, Low = 29, Close = 30, Volume = 3, OpenTime = DateTimeOffset.Parse("2026-01-02T10:00:00Z") },
            new() { High = 21, Low = 19, Close = 20, Volume = 2, OpenTime = DateTimeOffset.Parse("2026-01-01T10:00:00Z") },
            new() { High = 11, Low = 9, Close = 10, Volume = 1, OpenTime = DateTimeOffset.Parse("2026-01-01T09:00:00Z") }
        };
        var result = Indicators.Vwap(candles, (high, low, close, volume, _) =>
        {
            Assert.Equal([31d, 21d, 11d], high);
            Assert.Equal([29d, 19d, 9d], low);
            Assert.Equal([30d, 20d, 10d], close);
            Assert.Equal([3d, 2d, 1d], volume);
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(140d / 6, result.Vwap[0], 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Vwap(candles).IndicatorSignal);
    }

    [Fact]
    public void Vwap_WhenInputsAreNullEmptyOrDifferentLengths_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.Vwap(null!, [2], [2], [1]),
            Indicators.Vwap([3], null!, [2], [1]),
            Indicators.Vwap([3], [2], null!, [1]),
            Indicators.Vwap([3], [2], [2], null!),
            Indicators.Vwap([], [], [], []),
            Indicators.Vwap([3, 2], [2], [2], [1]),
            Indicators.Vwap([3], [2], [2, 1], [1]),
            Indicators.Vwap([3], [2], [2], [1, 1])
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }
}
