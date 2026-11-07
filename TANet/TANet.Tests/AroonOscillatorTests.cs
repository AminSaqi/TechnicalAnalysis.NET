using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class AroonOscillatorTests
{
    [Fact]
    public void AroonOscillator_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [13, 14, 11, 12, 10];
        double[] low = [10, 11, 7, 9, 8];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();

        var result = Indicators.AroonOscillator(high, low, period: 2);

        Assert.True(result.Success, result.Message);
        // Aroon Up is [50, 100, 50] and Aroon Down is [0, 50, 100].
        Assert.Equal([50d, 50d, -50d, double.NaN, double.NaN], result.AroonOscillator);
        Assert.True(result.AroonOscillator.HasValidData());
        Assert.Equal(3, result.AroonOscillator.CountValidData());
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
    }

    [Fact]
    public void AroonOscillator_ReturnsValuesMatchingOfficialTaLib()
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        const int period = 3;

        var result = Indicators.AroonOscillator(high, low, period);

        Assert.True(result.Success, result.Message);
        var expected = new double[high.Length];
        var range = TALib.Core.Default.Aroonosc(
            0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), period, expected);

        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.AroonOscillator.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.AroonOscillator.Take(range.Count));
        Assert.All(result.AroonOscillator.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.AroonOscillator.HasValidData());
        Assert.Equal(range.Count, result.AroonOscillator.CountValidData());
    }

    [Theory]
    [InlineData(true, 100d, IndicatorSignal.Buy)]
    [InlineData(false, -100d, IndicatorSignal.Sell)]
    public void AroonOscillator_WhenCurrentValueShowsTrend_ReturnsDefaultSignal(
        bool rising, double expectedValue, IndicatorSignal expectedSignal)
    {
        double[] high = rising ? [12, 11, 10] : [10, 11, 12];
        double[] low = rising ? [7, 6, 5] : [5, 6, 7];

        var result = Indicators.AroonOscillator(high, low, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedValue, result.AroonOscillator[0]);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void AroonOscillator_WhenUpAndDownAreEqual_ReturnsZeroAndStay()
    {
        var result = Indicators.AroonOscillator([10, 10, 10], [5, 5, 5], period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.AroonOscillator[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void AroonOscillator_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.AroonOscillator([12, 11], [7, 6], 2, values =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.AroonOscillator.Length);
        Assert.All(result.AroonOscillator, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.AroonOscillator.HasValidData());
        Assert.Equal(0, result.AroonOscillator.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void AroonOscillator_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[]? callbackValues = null;
        var result = Indicators.AroonOscillator([13, 14, 11, 12, 10], [10, 11, 7, 9, 8], 2,
            values =>
            {
                callbackValues = (double[])values.Clone();
                return IndicatorSignal.Sell;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackValues);
        Assert.Equal(result.AroonOscillator, callbackValues);
        Assert.Equal(50d, callbackValues[0]);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void AroonOscillator_WhenPeriodIsInvalid_ReturnsFailure()
    {
        var result = Indicators.AroonOscillator([12, 11, 10], [7, 6, 5], period: 1);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void AroonOscillator_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength)
    {
        var result = Indicators.AroonOscillator(new double[5], new double[lowLength], period: 2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void AroonOscillator_FromCandles_UsesNewestFirstHighAndLow()
    {
        var candles = new List<Candle>
        {
            new() { High = 13, Low = 10 },
            new() { High = 14, Low = 11 },
            new() { High = 11, Low = 7 },
            new() { High = 12, Low = 9 },
            new() { High = 10, Low = 8 }
        };

        var result = Indicators.AroonOscillator(candles, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([50d, 50d, -50d, double.NaN, double.NaN], result.AroonOscillator);
    }
}
