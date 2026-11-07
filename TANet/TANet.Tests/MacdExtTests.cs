using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class MacdExtTests
{
    [Fact]
    public void ExtendedMacd_ReturnsKnownSmaValuesInNewestFirstOrder()
    {
        double[] input = [5, 4, 3, 2, 1];
        var originalInput = (double[])input.Clone();

        var result = Indicators.ExtendedMacd(
            input, MovingAverageType.Sma, 2, MovingAverageType.Sma, 3, MovingAverageType.Sma, 2);

        Assert.True(result.Success, result.Message);
        // In each valid window, SMA(2) is 0.5 above SMA(3); the signal is also 0.5.
        Assert.Equal([0.5, 0.5, double.NaN, double.NaN, double.NaN], result.Macd);
        Assert.Equal([0.5, 0.5, double.NaN, double.NaN, double.NaN], result.Signal);
        Assert.Equal([0d, 0d, double.NaN, double.NaN, double.NaN], result.Histogram);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
        Assert.Equal(originalInput, input);
    }

    [Theory]
    [InlineData(MovingAverageType.Sma, MovingAverageType.Ema, MovingAverageType.Wma, TALib.MAType.SMA, TALib.MAType.EMA, TALib.MAType.WMA)]
    [InlineData(MovingAverageType.Ema, MovingAverageType.Wma, MovingAverageType.Kama, TALib.MAType.EMA, TALib.MAType.WMA, TALib.MAType.KAMA)]
    [InlineData(MovingAverageType.Wma, MovingAverageType.Kama, MovingAverageType.Tema, TALib.MAType.WMA, TALib.MAType.KAMA, TALib.MAType.TEMA)]
    [InlineData(MovingAverageType.Kama, MovingAverageType.Tema, MovingAverageType.Sma, TALib.MAType.KAMA, TALib.MAType.TEMA, TALib.MAType.SMA)]
    [InlineData(MovingAverageType.Tema, MovingAverageType.Sma, MovingAverageType.Ema, TALib.MAType.TEMA, TALib.MAType.SMA, TALib.MAType.EMA)]
    public void ExtendedMacd_ReturnsValuesMatchingOfficialTaLib(
        MovingAverageType fastType, MovingAverageType slowType, MovingAverageType signalType,
        TALib.MAType officialFastType, TALib.MAType officialSlowType, TALib.MAType officialSignalType)
    {
        double[] input =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10
        ];
        const int fastPeriod = 2;
        const int slowPeriod = 5;
        const int signalPeriod = 3;

        var result = Indicators.ExtendedMacd(
            input, fastType, fastPeriod, slowType, slowPeriod, signalType, signalPeriod);

        Assert.True(result.Success, result.Message);
        var expectedMacd = new double[input.Length];
        var expectedSignal = new double[input.Length];
        var expectedHistogram = new double[input.Length];
        var range = TALib.Core.Default.Macdext(
            0, input.Length - 1, input.Reverse().ToArray(),
            fastPeriod, officialFastType, slowPeriod, officialSlowType, signalPeriod, officialSignalType,
            expectedMacd, expectedSignal, expectedHistogram);

        Assert.True(range.Count > 0);

        void AssertSeries(double[] expected, double[] actual)
        {
            Assert.Equal(input.Length, actual.Length);
            Assert.Equal(expected.Take(range.Count).Reverse(), actual.Take(range.Count));
            Assert.All(actual.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
            Assert.True(actual.HasValidData());
            Assert.Equal(range.Count, actual.CountValidData());
        }

        AssertSeries(expectedMacd, result.Macd);
        AssertSeries(expectedSignal, result.Signal);
        AssertSeries(expectedHistogram, result.Histogram);
        for (var i = 0; i < range.Count; i++)
        {
            Assert.Equal(result.Macd[i] - result.Signal[i], result.Histogram[i], precision: 10);
        }
    }

    [Theory]
    [InlineData(1, IndicatorSignal.Buy)]
    [InlineData(-1, IndicatorSignal.Sell)]
    [InlineData(0, IndicatorSignal.Stay)]
    public void ExtendedMacd_WhenCurrentMacdChangesSign_ReturnsDefaultSignal(
        int trend, IndicatorSignal expectedSignal)
    {
        double[] input = trend switch
        {
            1 => [5, 4, 3, 2, 1],
            -1 => [1, 2, 3, 4, 5],
            _ => [3, 3, 3, 3, 3]
        };

        var result = Indicators.ExtendedMacd(
            input, MovingAverageType.Sma, 2, MovingAverageType.Sma, 3, MovingAverageType.Sma, 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(trend * 0.5, result.Macd[0], precision: 10);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void ExtendedMacd_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.ExtendedMacd(
            [2, 1], MovingAverageType.Ema, 2, MovingAverageType.Ema, 3, MovingAverageType.Ema, 2,
            (macd, signal, histogram) =>
            {
                invoked = true;
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        foreach (var series in new[] { result.Macd, result.Signal, result.Histogram })
        {
            Assert.Equal(2, series.Length);
            Assert.All(series, value => Assert.True(double.IsNaN(value)));
            Assert.False(series.HasValidData());
            Assert.Equal(0, series.CountValidData());
        }
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void ExtendedMacd_WhenDataIsValid_PassesAllSeriesToCustomSignalLogic()
    {
        double[]? callbackMacd = null;
        double[]? callbackSignal = null;
        double[]? callbackHistogram = null;
        var result = Indicators.ExtendedMacd(
            [5, 4, 3, 2, 1], MovingAverageType.Sma, 2, MovingAverageType.Sma, 3, MovingAverageType.Sma, 2,
            (macd, signal, histogram) =>
            {
                callbackMacd = (double[])macd.Clone();
                callbackSignal = (double[])signal.Clone();
                callbackHistogram = (double[])histogram.Clone();
                return IndicatorSignal.Sell;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackMacd);
        Assert.NotNull(callbackSignal);
        Assert.NotNull(callbackHistogram);
        Assert.Equal(result.Macd, callbackMacd);
        Assert.Equal(result.Signal, callbackSignal);
        Assert.Equal(result.Histogram, callbackHistogram);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(1, 3, 2)]
    [InlineData(2, 1, 2)]
    [InlineData(2, 3, 0)]
    public void ExtendedMacd_WhenPeriodIsInvalid_ReturnsFailure(int fastPeriod, int slowPeriod, int signalPeriod)
    {
        var result = Indicators.ExtendedMacd(
            [5, 4, 3, 2, 1], MovingAverageType.Sma, fastPeriod,
            MovingAverageType.Sma, slowPeriod, MovingAverageType.Sma, signalPeriod);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Macd_UsesTheSameEmaConfigurationAsExtendedMacd()
    {
        double[] input = [10, 12, 9, 11, 8, 7, 9, 6, 5, 4];

        var simple = Indicators.Macd(input, fastPeriod: 2, slowPeriod: 3, signalPeriod: 2);
        var extended = Indicators.ExtendedMacd(
            input, MovingAverageType.Ema, 2, MovingAverageType.Ema, 3, MovingAverageType.Ema, 2);

        Assert.True(simple.Success, simple.Message);
        Assert.True(extended.Success, extended.Message);
        Assert.Equal(extended.Macd, simple.Macd);
        Assert.Equal(extended.Signal, simple.Signal);
        Assert.Equal(extended.Histogram, simple.Histogram);
        Assert.Equal(extended.IndicatorSignal, simple.IndicatorSignal);
    }

    [Fact]
    public void ExtendedMacd_FromCandles_UsesRequestedCalculationBase()
    {
        var candles = new List<Candle>
        {
            new() { Open = 5, Close = 10 },
            new() { Open = 4, Close = 10 },
            new() { Open = 3, Close = 10 },
            new() { Open = 2, Close = 10 },
            new() { Open = 1, Close = 10 }
        };

        var result = Indicators.ExtendedMacd(
            candles, MovingAverageType.Sma, 2, MovingAverageType.Sma, 3, MovingAverageType.Sma, 2,
            IndicatorCalculationBase.Open);

        Assert.True(result.Success, result.Message);
        Assert.Equal([0.5, 0.5, double.NaN, double.NaN, double.NaN], result.Macd);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void ExtendedMacd_WhenPeriodsAreReversed_SwapsPeriodsAndTypes()
    {
        double[] input = [10, 12, 9, 11, 8, 7, 9, 6, 5, 4];

        var reversed = Indicators.ExtendedMacd(
            input, MovingAverageType.Wma, 5, MovingAverageType.Ema, 2, MovingAverageType.Sma, 2);
        var ordered = Indicators.ExtendedMacd(
            input, MovingAverageType.Ema, 2, MovingAverageType.Wma, 5, MovingAverageType.Sma, 2);

        Assert.True(reversed.Success, reversed.Message);
        Assert.True(ordered.Success, ordered.Message);
        Assert.Equal(ordered.Macd, reversed.Macd);
        Assert.Equal(ordered.Signal, reversed.Signal);
        Assert.Equal(ordered.Histogram, reversed.Histogram);
    }
}
