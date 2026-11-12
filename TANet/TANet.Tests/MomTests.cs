using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class MomTests
{
    [Fact]
    public void Mom_ReturnsKnownValuesNewestFirstWithoutChangingInput()
    {
        double[] input = [10, 8, 7, 4, 1];
        var original = (double[])input.Clone();

        var result = Indicators.Mom(input, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([3d, 4d, 6d, double.NaN, double.NaN], result.Mom);
        Assert.True(result.Mom.HasValidData());
        Assert.Equal(3, result.Mom.CountValidData());
        Assert.Equal(original, input);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public void Mom_MatchesOfficialTaLib(int period)
    {
        double[] input = [15, 13, 14, 10, 8, 9, 7, 4];
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Mom(0, input.Length - 1, input.Reverse().ToArray(), period, expected);

        var result = Indicators.Mom(input, period);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Mom.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Mom.Take(range.Count));
        Assert.All(result.Mom.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
    }

    [Theory]
    [InlineData(3d, IndicatorSignal.Buy)]
    [InlineData(1d, IndicatorSignal.Sell)]
    [InlineData(2d, IndicatorSignal.Stay)]
    public void Mom_DefaultSignalReflectsLatestMomentum(double latest, IndicatorSignal expected)
    {
        var result = Indicators.Mom([latest, 2], period: 1);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Mom_WhenHistoryIsInsufficient_ReturnsNaNsWithoutCallingSignal()
    {
        var called = false;
        var result = Indicators.Mom([2, 1], 2, _ =>
        {
            called = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Mom.Length);
        Assert.All(result.Mom, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Mom.HasValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Fact]
    public void Mom_WhenLatestValueIsNotFinite_DoesNotCallSignalEvenIfOlderValuesAreValid()
    {
        var called = false;
        var result = Indicators.Mom([double.NaN, 10, 8, 5], 1, _ =>
        {
            called = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.False(result.Mom.HasValidData());
        Assert.Equal(2d, result.Mom[1]);
        Assert.Equal(2, result.Mom.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Fact]
    public void Mom_CustomSignalReceivesTheCompleteNewestFirstOutput()
    {
        double[]? received = null;
        var result = Indicators.Mom([10, 8, 7, 4, 1], 2, values =>
        {
            received = (double[])values.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(result.Mom, received);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Mom_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.Mom([3, 2, 1], period);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Mom_WhenInputIsNullOrEmpty_ReturnsFailure()
    {
        var nullResult = Indicators.Mom((double[])null!, 1);
        var emptyResult = Indicators.Mom(Array.Empty<double>(), 1);

        Assert.False(nullResult.Success);
        Assert.False(emptyResult.Success);
        Assert.Equal(IndicatorSignal.Stay, nullResult.IndicatorSignal);
        Assert.Equal(IndicatorSignal.Stay, emptyResult.IndicatorSignal);
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Open, 1d)]
    [InlineData(IndicatorCalculationBase.High, 2d)]
    [InlineData(IndicatorCalculationBase.Low, 3d)]
    [InlineData(IndicatorCalculationBase.Close, 4d)]
    [InlineData(IndicatorCalculationBase.Volume, 5d)]
    public void Mom_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double expected)
    {
        var candles = new List<Candle>
        {
            new() { Open = 11, High = 22, Low = 33, Close = 44, Volume = 55 },
            new() { Open = 10, High = 20, Low = 30, Close = 40, Volume = 50 }
        };

        var result = Indicators.Mom(candles, 1, calculationBase, _ => IndicatorSignal.Sell);

        Assert.True(result.Success, result.Message);
        Assert.Equal([expected, double.NaN], result.Mom);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(4d, Indicators.Mom(candles, 1).Mom[0]);
    }
}
