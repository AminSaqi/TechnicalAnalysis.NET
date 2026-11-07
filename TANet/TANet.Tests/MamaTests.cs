using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class MamaTests
{
    [Theory]
    [InlineData(0.5, 0.05)]
    [InlineData(0.2, 0.1)]
    [InlineData(0.99, 0.01)]
    [InlineData(0.01, 0.01)]
    public void Mama_MatchesOfficialTaLibAndPreservesInput(double fastLimit, double slowLimit)
    {
        var input = Enumerable.Range(1, 90)
            .Select(value => 100 + value * 0.2 + Math.Sin(value * 0.3) * 5)
            .ToArray();
        var original = (double[])input.Clone();

        var result = Indicators.Mama(input, fastLimit, slowLimit);
        var expectedMama = new double[input.Length];
        var expectedFama = new double[input.Length];
        var range = TALib.Core.Default.Mama(
            0, input.Length - 1, input.Reverse().ToArray(),
            fastLimit, slowLimit, expectedMama, expectedFama);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Mama.Length);
        Assert.Equal(input.Length, result.Fama.Length);
        Assert.Equal(expectedMama.Take(range.Count).Reverse(), result.Mama.Take(range.Count));
        Assert.Equal(expectedFama.Take(range.Count).Reverse(), result.Fama.Take(range.Count));
        Assert.All(result.Mama.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.All(result.Fama.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Mama.HasValidData());
        Assert.True(result.Fama.HasValidData());
        Assert.Equal(range.Count, result.Mama.CountValidData());
        Assert.Equal(range.Count, result.Fama.CountValidData());
        Assert.Equal(original, input);
    }

    [Fact]
    public void Mama_WhenPricesAreZero_ReturnsZeroAndStay()
    {
        var input = new double[80];
        var lookback = TALib.Core.Default.MamaLookback(0.5, 0.05);

        var result = Indicators.Mama(input, fastLimit: 0.5, slowLimit: 0.05);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Mama.HasValidData());
        Assert.True(result.Fama.HasValidData());
        Assert.All(result.Mama.Take(input.Length - lookback), value => Assert.Equal(0d, value));
        Assert.All(result.Fama.Take(input.Length - lookback), value => Assert.Equal(0d, value));
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void Mama_DefaultSignalComparesLatestMamaAndFama(bool rising, IndicatorSignal expected)
    {
        var input = Enumerable.Range(1, 120).Select(value => 100d + value).ToArray();
        if (rising)
        {
            Array.Reverse(input);
        }

        var result = Indicators.Mama(input, 0.5, 0.05);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Mama.HasValidData());
        Assert.True(result.Fama.HasValidData());
        if (rising)
        {
            Assert.True(result.Mama[0] > result.Fama[0]);
        }
        else
        {
            Assert.True(result.Mama[0] < result.Fama[0]);
        }
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Mama_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var lookback = TALib.Core.Default.MamaLookback(0.5, 0.05);
        Assert.True(lookback > 0);
        var input = Enumerable.Range(1, lookback).Select(value => (double)value).ToArray();
        var invoked = false;

        var result = Indicators.Mama(input, 0.5, 0.05, (mama, fama) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Mama.Length);
        Assert.Equal(input.Length, result.Fama.Length);
        Assert.All(result.Mama, value => Assert.True(double.IsNaN(value)));
        Assert.All(result.Fama, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Mama.HasValidData());
        Assert.False(result.Fama.HasValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Mama_WithMinimumHistory_ReturnsOneValuePerSeries()
    {
        var lookback = TALib.Core.Default.MamaLookback(0.5, 0.05);
        var input = Enumerable.Range(1, lookback + 1).Select(value => (double)value).ToArray();

        var result = Indicators.Mama(input, 0.5, 0.05);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Mama.HasValidData());
        Assert.True(result.Fama.HasValidData());
        Assert.Equal(1, result.Mama.CountValidData());
        Assert.Equal(1, result.Fama.CountValidData());
        Assert.All(result.Mama[1..], value => Assert.True(double.IsNaN(value)));
        Assert.All(result.Fama[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Mama_PassesNewestFirstMamaThenFamaToCustomSignalLogic()
    {
        var input = Enumerable.Range(1, 80).Select(value => 100d + value).Reverse().ToArray();
        double[]? callbackMama = null;
        double[]? callbackFama = null;

        var result = Indicators.Mama(input, 0.5, 0.05, (mama, fama) =>
        {
            callbackMama = (double[])mama.Clone();
            callbackFama = (double[])fama.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackMama);
        Assert.NotNull(callbackFama);
        Assert.Equal(result.Mama, callbackMama);
        Assert.Equal(result.Fama, callbackFama);
        Assert.NotEqual(callbackMama[0], callbackFama[0]);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Mama_WhenLatestValuesAreNotFinite_DoesNotCallSignalLogic()
    {
        var input = Enumerable.Range(1, 80).Select(value => 100d + value).Reverse().ToArray();
        input[0] = double.NaN;
        var invoked = false;

        var result = Indicators.Mama(input, 0.5, 0.05, (mama, fama) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.True(double.IsNaN(result.Mama[0]));
        Assert.True(double.IsNaN(result.Fama[0]));
        Assert.True(result.Mama.CountValidData() > 0);
        Assert.True(result.Fama.CountValidData() > 0);
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0.009, 0.05)]
    [InlineData(1.0, 0.05)]
    [InlineData(0.5, 0.009)]
    [InlineData(0.5, 1.0)]
    public void Mama_WhenLimitsAreInvalid_ReturnsFailure(double fastLimit, double slowLimit)
    {
        var input = Enumerable.Range(1, 40).Select(value => (double)value).ToArray();
        var result = Indicators.Mama(input, fastLimit, slowLimit);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Mama_WhenInputIsNull_ReturnsFailure()
    {
        var result = Indicators.Mama((double[])null!, 0.5, 0.05);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Close)]
    [InlineData(IndicatorCalculationBase.Open)]
    [InlineData(IndicatorCalculationBase.High)]
    [InlineData(IndicatorCalculationBase.Low)]
    [InlineData(IndicatorCalculationBase.Volume)]
    public void Mama_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase)
    {
        var candles = Enumerable.Range(1, 80).Select(value => new Candle
        {
            Close = value + 10,
            Open = value + 20,
            High = value + 30,
            Low = value + 5,
            Volume = value + 50
        }).ToList();
        var selected = calculationBase switch
        {
            IndicatorCalculationBase.Open => candles.Select(c => c.Open).ToArray(),
            IndicatorCalculationBase.High => candles.Select(c => c.High).ToArray(),
            IndicatorCalculationBase.Low => candles.Select(c => c.Low).ToArray(),
            IndicatorCalculationBase.Volume => candles.Select(c => c.Volume).ToArray(),
            _ => candles.Select(c => c.Close).ToArray()
        };

        var result = Indicators.Mama(candles, 0.5, 0.05, calculationBase);
        var expected = Indicators.Mama(selected, 0.5, 0.05);

        Assert.True(result.Success, result.Message);
        Assert.True(expected.Success, expected.Message);
        Assert.Equal(expected.Mama, result.Mama);
        Assert.Equal(expected.Fama, result.Fama);
    }

    [Fact]
    public void Mama_FromCandles_DefaultsToCloseAndInvokesCustomSignal()
    {
        var candles = Enumerable.Range(1, 80)
            .Select(value => new Candle { Close = value + 10, Open = value + 20 })
            .ToList();
        double[]? callbackMama = null;
        double[]? callbackFama = null;

        var defaultResult = Indicators.Mama(candles, 0.5, 0.05);
        var arrayResult = Indicators.Mama(candles.Select(c => c.Close).ToArray(), 0.5, 0.05);
        var customResult = Indicators.Mama(candles, 0.5, 0.05, IndicatorCalculationBase.Open, (mama, fama) =>
        {
            callbackMama = (double[])mama.Clone();
            callbackFama = (double[])fama.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(defaultResult.Success, defaultResult.Message);
        Assert.True(arrayResult.Success, arrayResult.Message);
        Assert.Equal(arrayResult.Mama, defaultResult.Mama);
        Assert.Equal(arrayResult.Fama, defaultResult.Fama);
        Assert.True(customResult.Success, customResult.Message);
        Assert.NotNull(callbackMama);
        Assert.NotNull(callbackFama);
        Assert.Equal(customResult.Mama, callbackMama);
        Assert.Equal(customResult.Fama, callbackFama);
        Assert.Equal(IndicatorSignal.Sell, customResult.IndicatorSignal);
    }
}
