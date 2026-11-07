using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class WilliamsRTests
{
    [Fact]
    public void WilliamsR_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [14, 15, 12, 13, 11];
        double[] low = [12, 13, 10, 11, 9];
        double[] close = [13, 14, 11, 12, 10];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.WilliamsR(high, low, close, [], period: 3);

        Assert.True(result.Success, result.Message);
        // Chronological windows produce -50, -20 and -40.
        Assert.Equal([-40d, -20d, -50d, double.NaN, double.NaN], result.RangePercent);
        Assert.True(result.RangePercent.HasValidData());
        Assert.Equal(3, result.RangePercent.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(7)]
    public void WilliamsR_ReturnsValuesMatchingOfficialTaLib(int period)
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        double[] close = [18, 17, 14, 13, 15, 12, 11];

        var result = Indicators.WilliamsR(high, low, close, [], period);
        var expected = new double[high.Length];
        var range = TALib.Core.Default.Willr(
            0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            period, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.RangePercent.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.RangePercent.Take(range.Count));
        Assert.All(result.RangePercent.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.Equal(range.Count, result.RangePercent.CountValidData());
    }

    [Theory]
    [InlineData(10d, -90d, IndicatorSignal.Buy)]
    [InlineData(90d, -10d, IndicatorSignal.Sell)]
    [InlineData(50d, -50d, IndicatorSignal.Stay)]
    [InlineData(20d, -80d, IndicatorSignal.Stay)]
    [InlineData(80d, -20d, IndicatorSignal.Stay)]
    public void WilliamsR_UsesCurrentValueForDefaultSignal(
        double currentClose, double expectedValue, IndicatorSignal expectedSignal)
    {
        double[] high = [100, 100, 100];
        double[] low = [0, 0, 0];
        double[] close = [currentClose, 50, 50];

        var result = Indicators.WilliamsR(high, low, close, [], period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedValue, result.RangePercent[0]);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void WilliamsR_WhenPriceRangeIsZero_ReturnsZero()
    {
        double[] prices = [10, 10, 10];
        var result = Indicators.WilliamsR(prices, prices, prices, [], period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.RangePercent[0]);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void WilliamsR_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.WilliamsR([3, 2], [1, 0], [2, 1], [], 3, values =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.RangePercent.Length);
        Assert.All(result.RangePercent, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.RangePercent.HasValidData());
        Assert.Equal(0, result.RangePercent.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void WilliamsR_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        var invoked = false;
        var result = Indicators.WilliamsR([10, 10, 10], [0, 0, 0], [double.NaN, 5, 5], [], 2, values =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.True(double.IsNaN(result.RangePercent[0]));
        Assert.False(result.RangePercent.HasValidData());
        Assert.Equal(1, result.RangePercent.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void WilliamsR_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[]? callbackValues = null;
        var result = Indicators.WilliamsR(
            [14, 15, 12, 13, 11], [12, 13, 10, 11, 9], [13, 14, 11, 12, 10], [], 3,
            values =>
            {
                callbackValues = (double[])values.Clone();
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackValues);
        Assert.Equal(result.RangePercent, callbackValues);
        Assert.Equal(-40d, callbackValues[0]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void WilliamsR_WhenPeriodIsInvalid_ReturnsFailure(int period)
    {
        var result = Indicators.WilliamsR([3, 2], [1, 0], [2, 1], [], period);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4, 5)]
    [InlineData(6, 5)]
    [InlineData(5, 4)]
    [InlineData(5, 6)]
    public void WilliamsR_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength, int closeLength)
    {
        var result = Indicators.WilliamsR(new double[5], new double[lowLength], new double[closeLength], [], 3);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void WilliamsR_FromCandles_UsesNewestFirstHighLowAndClose()
    {
        var candles = new List<Candle>
        {
            new() { High = 14, Low = 12, Close = 13 },
            new() { High = 15, Low = 13, Close = 14 },
            new() { High = 12, Low = 10, Close = 11 },
            new() { High = 13, Low = 11, Close = 12 },
            new() { High = 11, Low = 9, Close = 10 }
        };

        var result = Indicators.WilliamsR(candles, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal([-40d, -20d, -50d, double.NaN, double.NaN], result.RangePercent);
    }
}
