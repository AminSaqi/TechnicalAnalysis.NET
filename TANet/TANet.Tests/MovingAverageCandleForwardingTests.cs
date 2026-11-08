using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using Xunit;

namespace TANet.Tests;

public class MovingAverageCandleForwardingTests
{
    private static List<Candle> SampleCandles() =>
        Enumerable.Range(1, 100).Select(value => new Candle
        {
            Open = 100 + value * 0.2 + Math.Sin(value * 0.3) * 5,
            Close = 70 + value,
            Volume = 1 + value % 7
        }).Reverse().ToList();

    [Fact]
    public void Alma_FromCandles_ForwardsCustomParametersAndSignal()
    {
        var candles = SampleCandles();
        var input = candles.Select(c => c.Open).ToArray();
        var fromCandles = Indicators.Alma(candles, 5, 3, 0.2, IndicatorCalculationBase.Open,
            (source, output) => IndicatorSignal.Sell);
        var fromArray = Indicators.Alma(input, 5, 3, 0.2, (source, output) => IndicatorSignal.Sell);
        var otherOffset = Indicators.Alma(input, 5, 3, 0.85);

        Assert.True(fromCandles.Success, fromCandles.Message);
        Assert.True(fromArray.Success, fromArray.Message);
        Assert.True(otherOffset.Success, otherOffset.Message);
        Assert.Equal(fromArray.Ma, fromCandles.Ma);
        Assert.NotEqual(otherOffset.Ma[0], fromCandles.Ma[0]);
        Assert.Equal(IndicatorSignal.Sell, fromCandles.IndicatorSignal);
    }

    [Fact]
    public void Mama_FromCandles_ForwardsCustomParametersAndBothSeries()
    {
        var candles = SampleCandles();
        var input = candles.Select(c => c.Open).ToArray();
        var fromCandles = Indicators.Mama(candles, 0.7, 0.12, IndicatorCalculationBase.Open,
            (mama, fama) => IndicatorSignal.Sell);
        var fromArray = Indicators.Mama(input, 0.7, 0.12, (mama, fama) => IndicatorSignal.Sell);
        var otherLimit = Indicators.Mama(input, 0.2, 0.12);

        Assert.True(fromCandles.Success, fromCandles.Message);
        Assert.True(fromArray.Success, fromArray.Message);
        Assert.True(otherLimit.Success, otherLimit.Message);
        Assert.Equal(fromArray.Mama, fromCandles.Mama);
        Assert.Equal(fromArray.Fama, fromCandles.Fama);
        Assert.NotEqual(otherLimit.Mama[0], fromCandles.Mama[0]);
        Assert.Equal(IndicatorSignal.Sell, fromCandles.IndicatorSignal);
    }

    [Fact]
    public void T3_FromCandles_ForwardsCustomFactorAndSignal()
    {
        var candles = SampleCandles();
        var input = candles.Select(c => c.Open).ToArray();
        var fromCandles = Indicators.T3(candles, 3, 0.2, IndicatorCalculationBase.Open,
            (source, output) => IndicatorSignal.Sell);
        var fromArray = Indicators.T3(input, 3, 0.2, (source, output) => IndicatorSignal.Sell);
        var otherFactor = Indicators.T3(input, 3, 0.9);

        Assert.True(fromCandles.Success, fromCandles.Message);
        Assert.True(fromArray.Success, fromArray.Message);
        Assert.True(otherFactor.Success, otherFactor.Message);
        Assert.Equal(fromArray.Ma, fromCandles.Ma);
        Assert.NotEqual(otherFactor.Ma[0], fromCandles.Ma[0]);
        Assert.Equal(IndicatorSignal.Sell, fromCandles.IndicatorSignal);
    }

    [Fact]
    public void Vidya_FromCandles_ForwardsCustomCmoPeriodAndSignal()
    {
        var candles = SampleCandles();
        var input = candles.Select(c => c.Open).ToArray();
        var fromCandles = Indicators.Vidya(candles, 5, 2, IndicatorCalculationBase.Open,
            (source, output) => IndicatorSignal.Sell);
        var fromArray = Indicators.Vidya(input, 5, 2, (source, output) => IndicatorSignal.Sell);
        var otherPeriod = Indicators.Vidya(input, 5, 7);

        Assert.True(fromCandles.Success, fromCandles.Message);
        Assert.True(fromArray.Success, fromArray.Message);
        Assert.True(otherPeriod.Success, otherPeriod.Message);
        Assert.Equal(fromArray.Ma, fromCandles.Ma);
        Assert.NotEqual(otherPeriod.Ma[0], fromCandles.Ma[0]);
        Assert.Equal(IndicatorSignal.Sell, fromCandles.IndicatorSignal);
    }

    [Fact]
    public void Vwma_FromCandles_ForwardsVolumeAndSignal()
    {
        var candles = SampleCandles();
        var input = candles.Select(c => c.Open).ToArray();
        var volume = candles.Select(c => c.Volume).ToArray();
        var fromCandles = Indicators.Vwma(candles, 5, IndicatorCalculationBase.Open,
            (source, output) => IndicatorSignal.Sell);
        var fromArray = Indicators.Vwma(input, volume, 5, (source, output) => IndicatorSignal.Sell);
        var uniformVolume = Indicators.Vwma(input, Enumerable.Repeat(1d, input.Length).ToArray(), 5);

        Assert.True(fromCandles.Success, fromCandles.Message);
        Assert.True(fromArray.Success, fromArray.Message);
        Assert.True(uniformVolume.Success, uniformVolume.Message);
        Assert.Equal(fromArray.Ma, fromCandles.Ma);
        Assert.NotEqual(uniformVolume.Ma[0], fromCandles.Ma[0]);
        Assert.Equal(IndicatorSignal.Sell, fromCandles.IndicatorSignal);
    }
}
