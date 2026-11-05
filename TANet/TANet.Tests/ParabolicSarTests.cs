using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class ParabolicSarTests
{
    [Fact]
    public void ParabolicSar_WithZeroAcceleration_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [15, 14, 13, 12, 11];
        double[] low = [13, 12, 11, 10, 9];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();

        var result = Indicators.ParabolicSar(high, low, acceleration: 0, maximum: 0.2);

        Assert.True(result.Success, result.Message);
        // The rising series starts at the oldest low; zero acceleration leaves SAR at 9.
        Assert.Equal([9d, 9d, 9d, 9d, double.NaN], result.Sar);
        Assert.True(result.Sar.HasValidData());
        Assert.Equal(4, result.Sar.CountValidData());
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
    }

    [Theory]
    [InlineData(0.02, 0.2)]
    [InlineData(0.1, 0.3)]
    [InlineData(0.0, 0.2)]
    public void ParabolicSar_ReturnsValuesMatchingOfficialTaLib(double acceleration, double maximum)
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];

        var result = Indicators.ParabolicSar(high, low, acceleration, maximum);

        Assert.True(result.Success, result.Message);
        var expected = new double[high.Length];
        var range = TALib.Core.Default.Sar(
            0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), acceleration, maximum, expected);

        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.Sar.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Sar.Take(range.Count));
        Assert.All(result.Sar.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Sar.HasValidData());
        Assert.Equal(range.Count, result.Sar.CountValidData());
    }

    [Theory]
    [InlineData(true, 9d, IndicatorSignal.Buy)]
    [InlineData(false, 12d, IndicatorSignal.Sell)]
    public void ParabolicSar_WhenCurrentPriceIsAboveOrBelowSar_ReturnsDefaultSignal(
        bool rising, double expectedSar, IndicatorSignal expectedSignal)
    {
        double[] high = rising ? [12, 11] : [11, 12];
        double[] low = rising ? [10, 9] : [9, 10];

        var result = Indicators.ParabolicSar(high, low, acceleration: 0.02, maximum: 0.2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedSar, result.Sar[0]);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void ParabolicSar_WhenCurrentPriceEqualsSar_ReturnsStay()
    {
        var result = Indicators.ParabolicSar([10, 10], [10, 10], acceleration: 0.02, maximum: 0.2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(10d, result.Sar[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void ParabolicSar_WhenHistoryIsTooShort_ReturnsNaNWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.ParabolicSar([11], [9], 0.02, 0.2, (high, low, sar) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Single(result.Sar);
        Assert.True(double.IsNaN(result.Sar[0]));
        Assert.False(result.Sar.HasValidData());
        Assert.Equal(0, result.Sar.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void ParabolicSar_WhenDataIsValid_PassesNewestFirstSeriesToCustomSignalLogic()
    {
        double[]? callbackHigh = null;
        double[]? callbackLow = null;
        double[]? callbackSar = null;
        double[] high = [15, 14, 13, 12, 11];
        double[] low = [13, 12, 11, 10, 9];

        var result = Indicators.ParabolicSar(high, low, 0, 0.2, (highValues, lowValues, sar) =>
        {
            callbackHigh = (double[])highValues.Clone();
            callbackLow = (double[])lowValues.Clone();
            callbackSar = (double[])sar.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackHigh);
        Assert.NotNull(callbackLow);
        Assert.NotNull(callbackSar);
        Assert.Equal(high, callbackHigh);
        Assert.Equal(low, callbackLow);
        Assert.Equal(result.Sar, callbackSar);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(-0.01, 0.2)]
    [InlineData(0.02, -0.1)]
    public void ParabolicSar_WhenParametersAreInvalid_ReturnsFailure(double acceleration, double maximum)
    {
        var result = Indicators.ParabolicSar([12, 11], [10, 9], acceleration, maximum);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void ParabolicSar_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength)
    {
        var result = Indicators.ParabolicSar(new double[5], new double[lowLength], 0.02, 0.2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void ParabolicSar_FromCandles_UsesNewestFirstHighAndLow()
    {
        var candles = new List<Candle>
        {
            new() { High = 15, Low = 13 },
            new() { High = 14, Low = 12 },
            new() { High = 13, Low = 11 },
            new() { High = 12, Low = 10 },
            new() { High = 11, Low = 9 }
        };

        var result = Indicators.ParabolicSar(candles, acceleration: 0, maximum: 0.2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([9d, 9d, 9d, 9d, double.NaN], result.Sar);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }
}
