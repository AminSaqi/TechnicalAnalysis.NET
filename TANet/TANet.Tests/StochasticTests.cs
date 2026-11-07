using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class StochasticTests
{
    [Fact]
    public void Stochastic_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [10, 10, 10, 10, 10];
        double[] low = [0, 0, 0, 0, 0];
        double[] close = [5, 4, 3, 2, 1];
        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();

        var result = Indicators.Stochastic(high, low, close, fastKPeriod: 2, slowKPeriod: 2, slowDPeriod: 2);

        Assert.True(result.Success, result.Message);
        // Chronological Fast K is 20, 30, 40, 50; aligned Slow K is 35, 45 and Slow D is 30, 40.
        Assert.Equal([45d, 35d, double.NaN, double.NaN, double.NaN], result.SlowK);
        Assert.Equal([40d, 30d, double.NaN, double.NaN, double.NaN], result.SlowD);
        Assert.True(result.SlowK.HasValidData());
        Assert.True(result.SlowD.HasValidData());
        Assert.Equal(2, result.SlowK.CountValidData());
        Assert.Equal(2, result.SlowD.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
    }

    [Theory]
    [InlineData(MovingAverageType.Sma, MovingAverageType.Ema, TALib.MAType.SMA, TALib.MAType.EMA)]
    [InlineData(MovingAverageType.Ema, MovingAverageType.Wma, TALib.MAType.EMA, TALib.MAType.WMA)]
    [InlineData(MovingAverageType.Wma, MovingAverageType.Kama, TALib.MAType.WMA, TALib.MAType.KAMA)]
    [InlineData(MovingAverageType.Kama, MovingAverageType.Tema, TALib.MAType.KAMA, TALib.MAType.TEMA)]
    [InlineData(MovingAverageType.Tema, MovingAverageType.Sma, TALib.MAType.TEMA, TALib.MAType.SMA)]
    public void Stochastic_ReturnsValuesMatchingOfficialTaLib(
        MovingAverageType slowKType, MovingAverageType slowDType,
        TALib.MAType officialSlowKType, TALib.MAType officialSlowDType)
    {
        double[] close =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10
        ];
        var high = close.Select(value => value + 2).ToArray();
        var low = close.Select(value => value - 3).ToArray();
        const int fastKPeriod = 3;
        const int slowKPeriod = 3;
        const int slowDPeriod = 3;

        var result = Indicators.Stochastic(
            high, low, close, fastKPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod);

        Assert.True(result.Success, result.Message);
        var expectedSlowK = new double[close.Length];
        var expectedSlowD = new double[close.Length];
        var range = TALib.Core.Default.Stoch(
            0, close.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            fastKPeriod, slowKPeriod, officialSlowKType, slowDPeriod, officialSlowDType,
            expectedSlowK, expectedSlowD);

        Assert.True(range.Count > 0);

        void AssertSeries(double[] expected, double[] actual)
        {
            Assert.Equal(close.Length, actual.Length);
            Assert.Equal(expected.Take(range.Count).Reverse(), actual.Take(range.Count));
            Assert.All(actual.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
            Assert.True(actual.HasValidData());
            Assert.Equal(range.Count, actual.CountValidData());
        }

        AssertSeries(expectedSlowK, result.SlowK);
        AssertSeries(expectedSlowD, result.SlowD);
    }

    [Theory]
    [InlineData(10d, IndicatorSignal.Buy)]
    [InlineData(90d, IndicatorSignal.Sell)]
    [InlineData(50d, IndicatorSignal.Stay)]
    [InlineData(20d, IndicatorSignal.Stay)]
    [InlineData(80d, IndicatorSignal.Stay)]
    public void Stochastic_WhenCurrentValuesCrossThresholds_ReturnsDefaultSignal(
        double closeValue, IndicatorSignal expectedSignal)
    {
        double[] high = [100, 100, 100, 100, 100];
        double[] low = [0, 0, 0, 0, 0];
        var close = Enumerable.Repeat(closeValue, high.Length).ToArray();

        var result = Indicators.Stochastic(high, low, close, fastKPeriod: 2, slowKPeriod: 2, slowDPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(closeValue, result.SlowK[0], precision: 10);
        Assert.Equal(closeValue, result.SlowD[0], precision: 10);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Fact]
    public void Stochastic_WhenRangeIsZero_ReturnsZero()
    {
        double[] prices = [10, 10, 10, 10, 10];

        var result = Indicators.Stochastic(prices, prices, prices, fastKPeriod: 2, slowKPeriod: 2, slowDPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(0d, result.SlowK[0]);
        Assert.Equal(0d, result.SlowD[0]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Stochastic_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Stochastic([10, 10], [0, 0], [5, 4], 2, 2, 2, (slowK, slowD) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        foreach (var series in new[] { result.SlowK, result.SlowD })
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
    public void Stochastic_WhenDataIsValid_PassesSlowKThenSlowDToCustomSignalLogic()
    {
        double[]? callbackSlowK = null;
        double[]? callbackSlowD = null;
        var result = Indicators.Stochastic(
            [10, 10, 10, 10, 10], [0, 0, 0, 0, 0], [5, 4, 3, 2, 1], 2, 2, 2,
            (slowK, slowD) =>
            {
                callbackSlowK = (double[])slowK.Clone();
                callbackSlowD = (double[])slowD.Clone();
                return IndicatorSignal.Sell;
            });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackSlowK);
        Assert.NotNull(callbackSlowD);
        Assert.Equal(result.SlowK, callbackSlowK);
        Assert.Equal(result.SlowD, callbackSlowD);
        Assert.Equal(45d, callbackSlowK[0], precision: 10);
        Assert.Equal(40d, callbackSlowD[0], precision: 10);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0, 2, 2)]
    [InlineData(2, 0, 2)]
    [InlineData(2, 2, 0)]
    public void Stochastic_WhenPeriodIsInvalid_ReturnsFailure(int fastKPeriod, int slowKPeriod, int slowDPeriod)
    {
        var result = Indicators.Stochastic(
            [10, 10, 10, 10], [0, 0, 0, 0], [4, 3, 2, 1], fastKPeriod, slowKPeriod, slowDPeriod);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4, 5)]
    [InlineData(6, 5)]
    [InlineData(5, 4)]
    [InlineData(5, 6)]
    public void Stochastic_WhenSeriesLengthsDiffer_ReturnsFailure(int lowLength, int closeLength)
    {
        var result = Indicators.Stochastic(new double[5], new double[lowLength], new double[closeLength], 2, 2, 2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void Stochastic_FromCandles_UsesNewestFirstHighLowAndClose()
    {
        var candles = new List<Candle>
        {
            new() { High = 10, Low = 0, Close = 5 },
            new() { High = 10, Low = 0, Close = 4 },
            new() { High = 10, Low = 0, Close = 3 },
            new() { High = 10, Low = 0, Close = 2 },
            new() { High = 10, Low = 0, Close = 1 }
        };

        var result = Indicators.Stochastic(candles, fastKPeriod: 2, slowKPeriod: 2, slowDPeriod: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal([45d, 35d, double.NaN, double.NaN, double.NaN], result.SlowK);
        Assert.Equal([40d, 30d, double.NaN, double.NaN, double.NaN], result.SlowD);
    }
}
