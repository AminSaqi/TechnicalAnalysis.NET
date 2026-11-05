using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class AroonTests
{
    [Fact]
    public void Aroon_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [13, 14, 11, 12, 10];
        double[] low = [10, 11, 7, 9, 8];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();

        var result = Indicators.Aroon(high, low, period: 2);

        Assert.True(result.Success, result.Message);
        // The newest window's highest high is one bar old and its lowest low is two bars old.
        Assert.Equal([50d, 100d, 50d, double.NaN, double.NaN], result.AroonUp);
        Assert.Equal([0d, 50d, 100d, double.NaN, double.NaN], result.AroonDown);
        Assert.True(result.AroonUp.HasValidData());
        Assert.True(result.AroonDown.HasValidData());
        Assert.Equal(3, result.AroonUp.CountValidData());
        Assert.Equal(3, result.AroonDown.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
    }

    [Fact]
    public void Aroon_ReturnsValuesMatchingOfficialTaLib()
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        const int period = 3;

        var result = Indicators.Aroon(high, low, period);

        Assert.True(result.Success, result.Message);
        var expectedDown = new double[high.Length];
        var expectedUp = new double[high.Length];
        var range = TALib.Core.Default.Aroon(
            0, high.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(),
            period, expectedDown, expectedUp);

        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.AroonUp.Length);
        Assert.Equal(high.Length, result.AroonDown.Length);
        Assert.Equal(expectedUp.Take(range.Count).Reverse(), result.AroonUp.Take(range.Count));
        Assert.Equal(expectedDown.Take(range.Count).Reverse(), result.AroonDown.Take(range.Count));
        Assert.All(result.AroonUp.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.All(result.AroonDown.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void Aroon_WhenCurrentValuesShowTrend_ReturnsDefaultSignal(
        bool rising, IndicatorSignal expectedSignal)
    {
        double[] high = rising ? [12, 11, 10] : [10, 11, 12];
        double[] low = rising ? [7, 6, 5] : [5, 6, 7];

        var result = Indicators.Aroon(high, low, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(rising ? 100d : 0d, result.AroonUp[0]);
        Assert.Equal(rising ? 0d : 100d, result.AroonDown[0]);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void Aroon_WhenExtremesAreRepeated_UsesTheMostRecentBar()
    {
        var result = Indicators.Aroon([10, 10, 10], [5, 5, 5], period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(100d, result.AroonUp[0]);
        Assert.Equal(100d, result.AroonDown[0]);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Aroon_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Aroon([12, 11], [7, 6], 2, (up, down) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.AroonUp.Length);
        Assert.Equal(2, result.AroonDown.Length);
        Assert.All(result.AroonUp, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.AroonDown, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.AroonUp.HasValidData());
        Assert.False(result.AroonDown.HasValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Aroon_WhenDataIsValid_PassesUpThenDownToCustomSignalLogic()
    {
        double[]? callbackUp = null;
        double[]? callbackDown = null;
        var result = Indicators.Aroon([13, 14, 11, 12, 10], [10, 11, 7, 9, 8], 2,
            (up, down) =>
            {
                callbackUp = (double[])up.Clone();
                callbackDown = (double[])down.Clone();
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackUp);
        Assert.NotNull(callbackDown);
        Assert.Equal(result.AroonUp, callbackUp);
        Assert.Equal(result.AroonDown, callbackDown);
        Assert.Equal(50d, callbackUp[0]);
        Assert.Equal(0d, callbackDown[0]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Aroon_WhenPeriodIsInvalid_ReturnsFailure()
    {
        var result = Indicators.Aroon([12, 11, 10], [7, 6, 5], period: 1);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4)]
    [InlineData(6)]
    public void Aroon_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength)
    {
        var result = Indicators.Aroon(new double[5], new double[lowLength], period: 2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void Aroon_FromCandles_UsesNewestFirstHighAndLow()
    {
        var candles = new List<Candle>
        {
            new() { High = 13, Low = 10 },
            new() { High = 14, Low = 11 },
            new() { High = 11, Low = 7 },
            new() { High = 12, Low = 9 },
            new() { High = 10, Low = 8 }
        };

        var result = Indicators.Aroon(candles, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([50d, 100d, 50d, double.NaN, double.NaN], result.AroonUp);
        Assert.Equal([0d, 50d, 100d, double.NaN, double.NaN], result.AroonDown);
    }
}
