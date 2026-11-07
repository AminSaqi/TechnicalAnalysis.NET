using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class MfiTests
{
    [Fact]
    public void Mfi_ReturnsKnownValuesInNewestFirstOrder()
    {
        double[] high = [14, 15, 12, 13, 11];
        double[] low = [12, 13, 10, 11, 9];
        double[] close = [13, 14, 11, 12, 10];
        double[] volume = [5, 4, 3, 2, 1];

        var originalHigh = (double[])high.Clone();
        var originalLow = (double[])low.Clone();
        var originalClose = (double[])close.Clone();
        var originalVolume = (double[])volume.Clone();

        var result = Indicators.Mfi(high, low, close, volume, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(high.Length, result.Mfi.Length);

        // The chronological positive/negative flow pairs are (24, 33), (56, 33), (56, 65).
        Assert.Equal(100d * 56 / 121, result.Mfi[0], precision: 10);
        Assert.Equal(100d * 56 / 89, result.Mfi[1], precision: 10);
        Assert.Equal(100d * 24 / 57, result.Mfi[2], precision: 10);

        Assert.All(result.Mfi[3..], value => Assert.True(double.IsNaN(value)));
        Assert.Equal(3, result.Mfi.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);

        Assert.Equal(originalHigh, high);
        Assert.Equal(originalLow, low);
        Assert.Equal(originalClose, close);
        Assert.Equal(originalVolume, volume);
    }

    [Fact]
    public void Mfi_ReturnsValuesMatchingOfficialTaLib()
    {
        double[] high = [20, 18, 19, 15, 17, 14, 13];
        double[] low = [16, 14, 13, 11, 12, 10, 9];
        double[] close = [18, 17, 14, 13, 15, 12, 11];
        double[] volume = [120, 80, 150, 90, 200, 110, 100];

        const int period = 3;

        var result = Indicators.Mfi(high, low, close, volume, period);

        Assert.True(result.Success, result.Message);

        var expected = new double[high.Length];

        var range = TALib.Core.Default.Mfi(
            0, 
            high.Length - 1,
            high.Reverse().ToArray(), 
            low.Reverse().ToArray(),
            close.Reverse().ToArray(), 
            volume.Reverse().ToArray(),
            period, 
            expected);

        Assert.True(range.Count > 0);
        Assert.Equal(high.Length, result.Mfi.Length);

        Assert.Equal(expected.Take(range.Count).Reverse(), result.Mfi.Take(range.Count));

        Assert.All(
            result.Mfi.Skip(range.Count), 
            value => Assert.True(double.IsNaN(value)));

        Assert.True(result.Mfi.HasValidData());
        Assert.Equal(range.Count, result.Mfi.CountValidData());
    }

    [Theory]
    [InlineData(false, 0d, IndicatorSignal.Buy)]
    [InlineData(true, 100d, IndicatorSignal.Sell)]
    public void Mfi_WhenPriceMovesInOneDirection_ReturnsDefaultSignal(
        bool rising, double expectedValue, IndicatorSignal expectedSignal)
    {
        double[] prices = rising ? [4, 3, 2, 1] : [1, 2, 3, 4];
        double[] volume = [10, 10, 10, 10];

        var result = Indicators.Mfi(prices, prices, prices, volume, period: 2);

        Assert.True(result.Success, result.Message);

        Assert.Equal(expectedValue, result.Mfi[0], precision: 10);
        Assert.Equal(expectedSignal, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(8d, 1d, 20d)]
    [InlineData(2d, 4d, 80d)]
    public void Mfi_AtDefaultSignalThresholds_ReturnsStay(
        double latestVolume, double previousVolume, double expectedValue)
    {
        double[] prices = [10, 20, 10];
        double[] volume = [latestVolume, previousVolume, 1];

        var result = Indicators.Mfi(prices, prices, prices, volume, period: 2);

        Assert.True(result.Success, result.Message);

        Assert.Equal(expectedValue, result.Mfi[0], precision: 10);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Mfi_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Mfi(
            [2, 1], 
            [2, 1], 
            [2, 1], 
            [10, 10], 
            3, 
            values =>
            {
                invoked = true;
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Mfi.Length);

        Assert.All(
            result.Mfi, 
            value => Assert.True(double.IsNaN(value)));

        Assert.False(result.Mfi.HasValidData());
        Assert.Equal(0, result.Mfi.CountValidData());

        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Mfi_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[]? callbackValues = null;
        var result = Indicators.Mfi(
            [14, 15, 12, 13, 11], 
            [12, 13, 10, 11, 9],
            [13, 14, 11, 12, 10], 
            [5, 4, 3, 2, 1], 2,
            values =>
            {
                callbackValues = (double[])values.Clone();
                return IndicatorSignal.Buy;
            });

        Assert.True(result.Success, result.Message);
        
        Assert.NotNull(callbackValues);
        Assert.Equal(result.Mfi, callbackValues);
        Assert.Equal(100d * 56 / 121, callbackValues[0], precision: 10);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Mfi_WhenPeriodIsInvalid_ReturnsFailure()
    {
        var result = Indicators.Mfi(
            [2, 1], 
            [2, 1], 
            [2, 1], 
            [10, 10], 
            period: 1);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(4, 5, 5)]
    [InlineData(6, 5, 5)]
    [InlineData(5, 4, 5)]
    [InlineData(5, 6, 5)]
    [InlineData(5, 5, 4)]
    [InlineData(5, 5, 6)]
    public void Mfi_WhenSeriesLengthsDiffer_ReturnsFailure(
        int lowLength, int closeLength, int volumeLength)
    {
        var result = Indicators.Mfi(
            new double[5], 
            new double[lowLength], 
            new double[closeLength], 
            new double[volumeLength], 
            2);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Contains("same length", result.Message);
    }

    [Fact]
    public void Mfi_FromCandles_UsesNewestFirstPricesAndVolume()
    {
        var candles = new List<Candle>
        {
            new() { High = 14, Low = 12, Close = 13, Volume = 5 },
            new() { High = 15, Low = 13, Close = 14, Volume = 4 },
            new() { High = 12, Low = 10, Close = 11, Volume = 3 },
            new() { High = 13, Low = 11, Close = 12, Volume = 2 },
            new() { High = 11, Low = 9, Close = 10, Volume = 1 }
        };

        var result = Indicators.Mfi(candles, period: 2);

        Assert.True(result.Success, result.Message);
        Assert.Equal(candles.Count, result.Mfi.Length);

        Assert.Equal(100d * 56 / 121, result.Mfi[0], precision: 10);

        Assert.All(
            result.Mfi[3..], 
            value => Assert.True(double.IsNaN(value)));
    }
}
