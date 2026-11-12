using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class HeikinAshiTests
{
    [Fact]
    public void HeikinAshi_ReturnsKnownCandlesNewestFirstWithoutMutatingInput()
    {
        double[] open = [15, 12, 10];
        double[] high = [19, 16, 13];
        double[] low = [14, 11, 9];
        double[] close = [18, 15, 12];
        var originals = new[] { open, high, low, close }.Select(x => (double[])x.Clone()).ToArray();

        var result = Indicators.HeikinAshi(open, high, low, close);

        Assert.True(result.Success, result.Message);
        Assert.Equal([12.25d, 11d, 11d], result.Open);
        Assert.Equal([19d, 16d, 13d], result.High);
        Assert.Equal([12.25d, 11d, 9d], result.Low);
        Assert.Equal([16.5d, 13.5d, 11d], result.Close);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originals[0], open);
        Assert.Equal(originals[1], high);
        Assert.Equal(originals[2], low);
        Assert.Equal(originals[3], close);
    }

    [Fact]
    public void HeikinAshi_MatchesOfficialTaLib()
    {
        double[] open = [16, 14, 15, 12, 10];
        double[] high = [20, 18, 17, 15, 13];
        double[] low = [14, 12, 11, 10, 8];
        double[] close = [19, 13, 12, 14, 11];
        var expected = Enumerable.Range(0, 4).Select(_ => new double[open.Length]).ToArray();
        var range = TALib.Core.Default.Ha(0, open.Length - 1,
            open.Reverse().ToArray(), high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            expected[0], expected[1], expected[2], expected[3]);

        var result = Indicators.HeikinAshi(open, high, low, close);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        double[][] actual = [result.Open, result.High, result.Low, result.Close];
        for (var i = 0; i < actual.Length; i++)
        {
            Assert.Equal(open.Length, actual[i].Length);
            Assert.Equal(expected[i].Take(range.Count).Reverse(), actual[i].Take(range.Count));
            Assert.All(actual[i].Skip(range.Count), value => Assert.True(double.IsNaN(value)));
            Assert.Equal(range.Count, actual[i].CountValidData());
        }
    }

    [Fact]
    public void HeikinAshi_SingleBarUsesRawOpenCloseMidpointAsSeed()
    {
        var result = Indicators.HeikinAshi([10], [15], [8], [14]);

        Assert.True(result.Success, result.Message);
        Assert.Equal([12d], result.Open);
        Assert.Equal([15d], result.High);
        Assert.Equal([8d], result.Low);
        Assert.Equal([11.75d], result.Close);
        Assert.True(result.Open.HasValidData());
    }

    [Fact]
    public void HeikinAshi_AddingNewerBarDoesNotChangeEarlierOutputs()
    {
        var full = Indicators.HeikinAshi([15, 12, 10], [19, 16, 13], [14, 11, 9], [18, 15, 12]);
        var older = Indicators.HeikinAshi([12, 10], [16, 13], [11, 9], [15, 12]);

        Assert.True(full.Success, full.Message);
        Assert.True(older.Success, older.Message);
        Assert.Equal(older.Open, full.Open[1..]);
        Assert.Equal(older.High, full.High[1..]);
        Assert.Equal(older.Low, full.Low[1..]);
        Assert.Equal(older.Close, full.Close[1..]);
    }

    [Fact]
    public void HeikinAshi_ChangingOldestBarChangesTheRecursiveSeed()
    {
        var full = Indicators.HeikinAshi([15, 12, 10], [19, 16, 13], [14, 11, 9], [18, 15, 12]);
        var shorter = Indicators.HeikinAshi([15, 12], [19, 16], [14, 11], [18, 15]);

        Assert.True(full.Success, full.Message);
        Assert.True(shorter.Success, shorter.Message);
        Assert.Equal(12.25d, full.Open[0]);
        Assert.Equal(13.5d, shorter.Open[0]);
    }

    [Fact]
    public void HeikinAshi_CustomSignalReceivesAllFourOutputSeries()
    {
        double[][]? received = null;
        var result = Indicators.HeikinAshi([15, 12, 10], [19, 16, 13], [14, 11, 9], [18, 15, 12],
            (open, high, low, close) =>
            {
                received = [open, high, low, close];
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(result.Open, received[0]);
        Assert.Equal(result.High, received[1]);
        Assert.Equal(result.Low, received[2]);
        Assert.Equal(result.Close, received[3]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void HeikinAshi_WhenLatestOutputIsNotFinite_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.HeikinAshi([10, 9], [double.PositiveInfinity, 12], [8, 7], [11, 10],
            (_, _, _, _) => { called = true; return IndicatorSignal.Buy; });

        Assert.True(result.Success, result.Message);
        Assert.False(result.High.HasValidData());
        Assert.True(double.IsFinite(result.High[1]));
        Assert.False(called);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void HeikinAshi_FromCandlesUsesOhlcAndForwardsCallback()
    {
        var candles = new List<Candle>
        {
            new() { Open = 15, High = 19, Low = 14, Close = 18, Volume = 7 },
            new() { Open = 12, High = 16, Low = 11, Close = 15, Volume = 6 },
            new() { Open = 10, High = 13, Low = 9, Close = 12, Volume = 5 }
        };
        var result = Indicators.HeikinAshi(candles, (open, high, low, close) =>
        {
            Assert.Equal([12.25d, 11d, 11d], open);
            Assert.Equal([19d, 16d, 13d], high);
            Assert.Equal([12.25d, 11d, 9d], low);
            Assert.Equal([16.5d, 13.5d, 11d], close);
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.HeikinAshi(candles).IndicatorSignal);
    }

    [Fact]
    public void HeikinAshi_WhenInputsAreNullEmptyOrDifferentLengths_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.HeikinAshi(null!, [2], [1], [2]),
            Indicators.HeikinAshi([2], null!, [1], [2]),
            Indicators.HeikinAshi([2], [3], null!, [2]),
            Indicators.HeikinAshi([2], [3], [1], null!),
            Indicators.HeikinAshi([], [], [], []),
            Indicators.HeikinAshi([2, 1], [3], [1], [2]),
            Indicators.HeikinAshi([2], [3, 2], [1], [2]),
            Indicators.HeikinAshi([2], [3], [1, 0], [2]),
            Indicators.HeikinAshi([2], [3], [1], [2, 1])
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }
}
