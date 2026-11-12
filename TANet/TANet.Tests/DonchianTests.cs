using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class DonchianTests
{
    [Fact]
    public void Donchian_ReturnsKnownBandsNewestFirstWithoutChangingInputs()
    {
        double[] high = [13, 12, 11, 10, 9];
        double[] low = [8, 7, 6, 5, 4];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();

        var result = Indicators.Donchian(high, low, 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal([13d, 12d, 11d, double.NaN, double.NaN], result.UpperBand);
        Assert.Equal([9.5d, 8.5d, 7.5d, double.NaN, double.NaN], result.MiddleBand);
        Assert.Equal([6d, 5d, 4d, double.NaN, double.NaN], result.LowerBand);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    public void Donchian_MatchesOfficialTaLib(int period)
    {
        double[] high = [15, 14, 18, 12, 13, 11, 10];
        double[] low = [10, 9, 12, 7, 8, 6, 5];
        var upper = new double[high.Length];
        var middle = new double[high.Length];
        var lower = new double[high.Length];
        var range = TALib.Core.Default.Donchian(0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), period, upper, middle, lower);

        var result = Indicators.Donchian(high, low, period);

        Assert.True(result.Success, result.Message);
        void AssertBand(double[] expected, double[] actual)
        {
            Assert.Equal(high.Length, actual.Length);
            Assert.Equal(expected.Take(range.Count).Reverse(), actual.Take(range.Count));
            Assert.All(actual.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
            Assert.True(actual.HasValidData());
            Assert.Equal(range.Count, actual.CountValidData());
        }
        AssertBand(upper, result.UpperBand);
        AssertBand(middle, result.MiddleBand);
        AssertBand(lower, result.LowerBand);
    }

    [Fact]
    public void Donchian_WhenHistoryIsInsufficient_ReturnsNaNsWithoutCallingSignal()
    {
        var called = false;
        var result = Indicators.Donchian([5, 4], [2, 1], 3, (_, _, _, _, _) =>
        {
            called = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        foreach (var band in new[] { result.UpperBand, result.MiddleBand, result.LowerBand })
        {
            Assert.Equal(2, band.Length);
            Assert.All(band, value => Assert.True(double.IsNaN(value)));
            Assert.False(band.HasValidData());
        }
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Fact]
    public void Donchian_WhenLatestBandIsNotFinite_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Donchian([double.PositiveInfinity, 12, 11], [8, 7, 6], 2,
            (_, _, _, _, _) =>
            {
                called = true;
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.False(result.UpperBand.HasValidData());
        Assert.True(double.IsFinite(result.UpperBand[1]));
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Fact]
    public void Donchian_WithOneValidValue_CallsSignalWithPreviousBandStillNaN()
    {
        var called = false;
        var result = Indicators.Donchian([12, 11], [7, 6], 2, (_, _, upper, _, _) =>
        {
            called = true;
            Assert.Equal(12d, upper[0]);
            Assert.True(double.IsNaN(upper[1]));
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.True(called);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Donchian_CustomSignalReceivesInputsAndBandsInNewestFirstOrder()
    {
        double[] high = [13, 12, 11, 10, 9];
        double[] low = [8, 7, 6, 5, 4];
        double[][]? received = null;

        var result = Indicators.Donchian(high, low, 3, (h, l, upper, middle, lower) =>
        {
            received = [h, l, upper, middle, lower];
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(high, received[0]);
        Assert.Equal(low, received[1]);
        Assert.Equal(result.UpperBand, received[2]);
        Assert.Equal(result.MiddleBand, received[3]);
        Assert.Equal(result.LowerBand, received[4]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Donchian_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.Donchian([4, 3, 2], [3, 2, 1], period);
        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Donchian_WhenInputsAreNullEmptyOrDifferentLengths_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.Donchian(null!, [1, 2], 2),
            Indicators.Donchian([1, 2], null!, 2),
            Indicators.Donchian([], [], 2),
            Indicators.Donchian([3, 2, 1], [2, 1], 2),
            Indicators.Donchian([2, 1], [3, 2, 1], 2)
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }

    [Fact]
    public void Donchian_FromCandles_UsesHighAndLowAndForwardsSignal()
    {
        var candles = new List<Candle>
        {
            new() { High = 13, Low = 8 },
            new() { High = 12, Low = 7 },
            new() { High = 11, Low = 6 }
        };
        var result = Indicators.Donchian(candles, 2, (high, low, _, _, _) =>
        {
            Assert.Equal([13d, 12d, 11d], high);
            Assert.Equal([8d, 7d, 6d], low);
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal([13d, 12d, double.NaN], result.UpperBand);
        Assert.Equal([7d, 6d, double.NaN], result.LowerBand);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, Indicators.Donchian(candles, 2).IndicatorSignal);
    }
}
