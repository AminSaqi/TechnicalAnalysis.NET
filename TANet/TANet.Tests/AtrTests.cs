using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class AtrTests
{
    [Fact]
    public void Atr_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [16, 16, 14, 13, 11];
        double[] low = [13, 10, 10, 10, 9];
        double[] close = [14, 15, 11, 12, 10];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.Atr(high, low, close, period: 2);

        Assert.True(result.Success, result.Message);

        // Chronological true ranges are 3, 4, 6, 3; Wilder's ATR is 3.5, 4.75, 3.875.
        Assert.Equal([3.875, 4.75, 3.5, double.NaN, double.NaN], result.Atr);

        Assert.True(result.Atr.HasValidData());
        Assert.Equal(3, result.Atr.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Atr_ReturnsValuesMatchingOfficialTaLib()
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        double[] close = [18, 17, 14, 13, 15, 12, 11];
        const int period = 3;

        var result = Indicators.Atr(high, low, close, period);

        Assert.True(result.Success, result.Message);

        var expected = new double[high.Length];
        var chronologicalHigh = high.Reverse().ToArray();
        var chronologicalLow = low.Reverse().ToArray();
        var chronologicalClose = close.Reverse().ToArray();

        var range = TALib.Core.Default.Atr(
            0, 
            high.Length - 1, 
            chronologicalHigh,
            chronologicalLow,
            chronologicalClose, 
            period, 
            expected);

        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.Atr.Length);

        var expectedValues = expected.Take(range.Count).Reverse().ToArray();
        Assert.Equal(expectedValues, result.Atr[..range.Count]);

        Assert.All(
            result.Atr.Skip(range.Count), 
            value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Atr_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Atr(
            [13, 11], 
            [10, 9], 
            [12, 10], 
            2, 
            values =>
            {
                invoked = true;
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Atr.Length);

        Assert.All(result.Atr, value => Assert.True(double.IsNaN(value)));

        Assert.False(result.Atr.HasValidData());
        Assert.Equal(0, result.Atr.CountValidData());

        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Atr_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[]? callbackValues = null;
        var result = Indicators.Atr(
            [16, 16, 14, 13, 11], 
            [13, 10, 10, 10, 9], 
            [14, 15, 11, 12, 10], 
            2,
            values =>
            {
                callbackValues = (double[])values.Clone();
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);

        Assert.NotNull(callbackValues);
        Assert.Equal([3.875, 4.75, 3.5, double.NaN, double.NaN], callbackValues);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Atr_WhenPeriodIsInvalid_ReturnsFailure()
    {
        var result = Indicators.Atr([13, 11], [10, 9], [12, 10], period: 0);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4, 5)]
    [InlineData(6, 5)]
    [InlineData(5, 4)]
    [InlineData(5, 6)]
    public void Atr_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength, int closeLength)
    {
        var result = Indicators.Atr(new double[5], new double[lowLength], new double[closeLength], 2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void Atr_FromCandles_UsesNewestFirstHighLowAndClose()
    {
        var candles = new List<Candle>
        {
            new() { High = 16, Low = 13, Close = 14 },
            new() { High = 16, Low = 10, Close = 15 },
            new() { High = 14, Low = 10, Close = 11 },
            new() { High = 13, Low = 10, Close = 12 },
            new() { High = 11, Low = 9, Close = 10 }
        };

        var result = Indicators.Atr(candles, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([3.875, 4.75, 3.5, double.NaN, double.NaN], result.Atr);
    }
}
