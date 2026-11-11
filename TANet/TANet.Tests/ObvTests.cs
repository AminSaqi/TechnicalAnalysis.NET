using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;

namespace TANet.Tests;

public class ObvTests
{
    [Fact]
    public void Obv_ReturnsKnownValuesNewestFirstWithoutChangingInputs()
    {
        double[] price = [13, 12, 12, 11, 10];
        double[] volume = [5, 4, 3, 2, 1];
        var originalPrice = (double[])price.Clone();
        var originalVolume = (double[])volume.Clone();

        var result = Indicators.Obv(price, volume);

        Assert.True(result.Success, result.Message);
        Assert.Equal([11d, 6d, 6d, 3d, 1d], result.Obv);
        Assert.True(result.Obv.HasValidData());
        Assert.Equal(5, result.Obv.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.Equal(originalPrice, price);
        Assert.Equal(originalVolume, volume);
    }

    [Fact]
    public void Obv_MatchesOfficialTaLib()
    {
        double[] price = [13, 12, 15, 15, 14, 11, 12, 10];
        double[] volume = [5, 7, 0, 2, 8, 4, 3, 6];
        var expected = new double[price.Length];
        var range = TALib.Core.Default.Obv(0, price.Length - 1,
            price.Reverse().ToArray(), volume.Reverse().ToArray(), expected);

        var result = Indicators.Obv(price, volume);

        Assert.True(result.Success, result.Message);
        Assert.Equal(price.Length, result.Obv.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Obv.Take(range.Count));
        Assert.All(result.Obv.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Obv_WhenPriceFalls_SubtractsVolumeAndCanBecomeNegative()
    {
        var result = Indicators.Obv([7, 8, 9], [5, 2, 1]);

        Assert.True(result.Success, result.Message);
        Assert.Equal([-6d, -1d, 1d], result.Obv);
    }

    [Fact]
    public void Obv_SingleBarStartsAtItsVolume()
    {
        var result = Indicators.Obv([42], [7]);

        Assert.True(result.Success, result.Message);
        Assert.Equal([7d], result.Obv);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Obv_AnchorIsTheOldestSuppliedBar()
    {
        var fullRange = Indicators.Obv([13, 12, 11, 10], [4, 3, 2, 1]);
        var laterRange = Indicators.Obv([13, 12, 11], [4, 3, 2]);

        Assert.True(fullRange.Success, fullRange.Message);
        Assert.True(laterRange.Success, laterRange.Message);
        Assert.Equal(10d, fullRange.Obv[0]);
        Assert.Equal(9d, laterRange.Obv[0]);
    }

    [Fact]
    public void Obv_CustomSignalReceivesPriceVolumeAndOutput()
    {
        double[] price = [13, 12, 11];
        double[] volume = [3, 2, 1];
        double[][]? received = null;
        var result = Indicators.Obv(price, volume, (p, v, output) =>
        {
            received = [p, v, output];
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(received);
        Assert.Equal(price, received[0]);
        Assert.Equal(volume, received[1]);
        Assert.Equal(result.Obv, received[2]);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Obv_WhenLatestValueIsNotFinite_DoesNotCallSignal()
    {
        var called = false;
        var result = Indicators.Obv([3, 2, 1], [double.NaN, 1, 1], (_, _, _) =>
        {
            called = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.False(result.Obv.HasValidData());
        Assert.True(double.IsFinite(result.Obv[1]));
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(called);
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Open, 6d)]
    [InlineData(IndicatorCalculationBase.High, 6d)]
    [InlineData(IndicatorCalculationBase.Low, -4d)]
    [InlineData(IndicatorCalculationBase.Close, 2d)]
    [InlineData(IndicatorCalculationBase.Volume, 6d)]
    public void Obv_FromCandles_UsesSelectedPriceAndVolume(IndicatorCalculationBase calculationBase, double expected)
    {
        var candles = new List<Candle>
        {
            new() { Open = 3, High = 4, Low = 0, Close = 2, Volume = 3 },
            new() { Open = 2, High = 3, Low = 1, Close = 1, Volume = 2 },
            new() { Open = 1, High = 2, Low = 2, Close = 2, Volume = 1 }
        };

        var result = Indicators.Obv(candles, calculationBase, (_, _, _) => IndicatorSignal.Sell);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected, result.Obv[0]);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
        Assert.Equal(2d, Indicators.Obv(candles).Obv[0]);
    }

    [Fact]
    public void Obv_FromCandlesWithCustomSignal_UsesCloseByDefault()
    {
        var candles = new List<Candle>
        {
            new() { Close = 5, Volume = 3 },
            new() { Close = 4, Volume = 2 }
        };
        var result = Indicators.Obv(candles, (price, volume, output) =>
        {
            Assert.Equal([5d, 4d], price);
            Assert.Equal([3d, 2d], volume);
            Assert.Equal([5d, 2d], output);
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Obv_WhenInputsAreInvalid_ReturnsFailure()
    {
        var results = new[]
        {
            Indicators.Obv(null!, [1]),
            Indicators.Obv([1], null!),
            Indicators.Obv([], []),
            Indicators.Obv([1, 2], [1]),
            Indicators.Obv([1], [1, 2])
        };
        Assert.All(results, result =>
        {
            Assert.False(result.Success);
            Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        });
    }
}
