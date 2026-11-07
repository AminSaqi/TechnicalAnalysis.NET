using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class CciTests
{
    [Fact]
    public void Cci_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [14, 15, 12, 13, 11];
        double[] low = [12, 13, 10, 11, 9];
        double[] close = [13, 14, 11, 12, 10];

        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.Cci(high, low, close, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(high.Length, result.Cci.Length);

        // Chronological typical prices are 10, 12, 11, 14, 13.
        Assert.Equal(20d, result.Cci[0], precision: 10);
        Assert.Equal(100d, result.Cci[1], precision: 10);
        Assert.Equal(0d, result.Cci[2], precision: 10);

        Assert.All(
            result.Cci[3..], 
            value => Assert.True(double.IsNaN(value)));

        Assert.True(result.Cci.HasValidData());
        Assert.Equal(3, result.Cci.CountValidData());

        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);

        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
    }

    [Fact]
    public void Cci_ReturnsValuesMatchingOfficialTaLib()
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        double[] close = [18, 17, 14, 13, 15, 12, 11];
        const int period = 3;

        var result = Indicators.Cci(high, low, close, period);

        Assert.True(result.Success, result.Message);

        var expected = new double[high.Length];
        var range = TALib.Core.Default.Cci(
            0, 
            high.Length - 1,
            high.Reverse().ToArray(), 
            low.Reverse().ToArray(), 
            close.Reverse().ToArray(),
            period, 
            expected);

        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.Cci.Length);

        Assert.Equal(expected.Take(range.Count).Reverse(), result.Cci.Take(range.Count));

        Assert.All(
            result.Cci.Skip(range.Count), 
            value => Assert.True(double.IsNaN(value)));

        Assert.True(result.Cci.HasValidData());
        Assert.Equal(range.Count, result.Cci.CountValidData());
    }

    [Theory]
    [InlineData(true, 120d, IndicatorSignal.Buy)]
    [InlineData(false, -120d, IndicatorSignal.Sell)]
    public void Cci_WhenCurrentValueIsOutsideThresholds_ReturnsDefaultSignal(
        bool rising, 
        double expectedValue, 
        IndicatorSignal expectedSignal)
    {
        double[] prices = rising ? [5, 3, 2, 1] : [1, 3, 4, 5];
        var high = prices.Select(value => value + 1).ToArray();
        var low = prices.Select(value => value - 1).ToArray();

        var result = Indicators.Cci(high, low, prices, period: 4);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedValue, result.Cci[0], precision: 10);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void Cci_WhenPricesAreConstant_ReturnsZeroAndStay()
    {
        double[] prices = [10, 10, 10, 10];

        var result = Indicators.Cci(prices, prices, prices, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.Cci[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Cci_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Cci(
            [3, 2], 
            [1, 0], 
            [2, 1], 
            3, 
            values =>
            {
                invoked = true;
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Cci.Length);

        Assert.All(
            result.Cci, 
            value => Assert.True(double.IsNaN(value)));

        Assert.False(result.Cci.HasValidData());
        Assert.Equal(0, result.Cci.CountValidData());

        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Cci_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[]? callbackValues = null;
        var result = Indicators.Cci(
            [14, 15, 12, 13, 11], 
            [12, 13, 10, 11, 9], 
            [13, 14, 11, 12, 10], 
            3,
            values =>
            {
                callbackValues = (double[])values.Clone();
                return IndicatorSignal.Sell;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackValues);

        Assert.Equal(result.Cci, callbackValues);
        Assert.Equal(20d, callbackValues[0], precision: 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Cci_WhenPeriodIsInvalid_ReturnsFailure()
    {
        var result = Indicators.Cci(
            [3, 2], 
            [1, 0], 
            [2, 1], 
            period: 1);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4, 5)]
    [InlineData(6, 5)]
    [InlineData(5, 4)]
    [InlineData(5, 6)]
    public void Cci_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength, int closeLength)
    {
        var result = Indicators.Cci(
            new double[5], 
            new double[lowLength], 
            new double[closeLength], 
            3);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void Cci_FromCandles_UsesNewestFirstHighLowAndClose()
    {
        var candles = new List<Candle>
        {
            new() { High = 14, Low = 12, Close = 13 },
            new() { High = 15, Low = 13, Close = 14 },
            new() { High = 12, Low = 10, Close = 11 },
            new() { High = 13, Low = 11, Close = 12 },
            new() { High = 11, Low = 9, Close = 10 }
        };

        var result = Indicators.Cci(candles, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(candles.Count, result.Cci.Length);

        Assert.Equal(20d, result.Cci[0], precision: 10);

        Assert.All(
            result.Cci[3..], 
            value => Assert.True(double.IsNaN(value)));
    }
}
