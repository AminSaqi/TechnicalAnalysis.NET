using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;

namespace TANet.Tests;

public class AlmaTests
{
    [Fact]
    public void Alma_ReturnsKnownGaussianWeightedValueInNewestFirstOrder()
    {
        double[] input = [9, 4, 8, 3, 7, 2];
        var original = (double[])input.Clone();

        var result = Indicators.Alma(input, period: 3, sigma: 3, offset: 0.5);

        // The newest chronological window is [8, 4, 9] with weights [e^-0.5, 1, e^-0.5].
        var edgeWeight = Math.Exp(-0.5);
        var expectedLatest = (17 * edgeWeight + 4) / (2 * edgeWeight + 1);
        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expectedLatest, result.Ma[0], precision: 10);
        Assert.All(result.Ma[4..], value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(4, result.Ma.CountValidData());
        Assert.Equal(original, input);
    }

    [Fact]
    public void Alma_FloorsTheGaussianPeakForAnEvenPeriod()
    {
        double[] input = [9, 4, 8, 3, 7, 2];

        var result = Indicators.Alma(input, period: 4, sigma: 4, offset: 0.5);

        // The newest chronological window is [3, 8, 4, 9], with the peak at floor(0.5 * 3) = 1.
        var adjacentWeight = Math.Exp(-0.5);
        var farWeight = Math.Exp(-2);
        var expectedLatest = (7 * adjacentWeight + 8 + 9 * farWeight) / (2 * adjacentWeight + 1 + farWeight);
        Assert.True(result.Success, result.Message);
        Assert.Equal(expectedLatest, result.Ma[0], precision: 10);
        Assert.Equal(3, result.Ma.CountValidData());
        Assert.All(result.Ma[3..], value => Assert.True(double.IsNaN(value)));
    }

    [Theory]
    [InlineData(1, 6d, 0.85)]
    [InlineData(3, 0.01, 0d)]
    [InlineData(4, 4d, 0.5)]
    [InlineData(9, 6d, 0.85)]
    [InlineData(10, 10d, 1d)]
    public void Alma_MatchesOfficialTaLib(int period, double sigma, double offset)
    {
        double[] input =
        [
            32, 30, 31, 29, 28, 26, 27, 24, 25, 23, 22, 24,
            21, 19, 20, 17, 18, 16, 14, 15, 12, 13, 11, 10, 9
        ];

        var result = Indicators.Alma(input, period, sigma, offset);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Alma(
            0, input.Length - 1, input.Reverse().ToArray(), period, sigma, offset, expected);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(range.Count, result.Ma.CountValidData());
    }

    [Fact]
    public void Alma_WhenPeriodIsOne_ReturnsInputAndStay()
    {
        double[] input = [5, 4, 3, 2, 1];

        var result = Indicators.Alma(input, period: 1, sigma: 6, offset: 0.85);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(true, IndicatorSignal.Buy)]
    [InlineData(false, IndicatorSignal.Sell)]
    public void Alma_DefaultSignalComparesLatestInputAndOutput(bool rising, IndicatorSignal expected)
    {
        double[] input = rising ? [5, 4, 3, 2, 1] : [1, 2, 3, 4, 5];

        var result = Indicators.Alma(input, period: 3, sigma: 3, offset: 0.5);

        Assert.True(result.Success, result.Message);
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(expected, result.IndicatorSignal);
    }

    [Fact]
    public void Alma_WhenHistoryIsTooShort_ReturnsNaNsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Alma([2, 1], 3, 6, 0.85, (input, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(0, result.Ma.CountValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Alma_WithMinimumHistory_ReturnsOneValue()
    {
        var result = Indicators.Alma([8, 6, 2], period: 3, sigma: 3, offset: 0.5);

        var edgeWeight = Math.Exp(-0.5);
        var expected = (10 * edgeWeight + 6) / (2 * edgeWeight + 1);
        Assert.True(result.Success, result.Message);
        Assert.Equal(expected, result.Ma[0], precision: 10);
        Assert.Equal(1, result.Ma.CountValidData());
        Assert.All(result.Ma[1..], value => Assert.True(double.IsNaN(value)));
    }

    [Fact]
    public void Alma_WhenDataIsValid_PassesNewestFirstValuesToCustomSignalLogic()
    {
        double[] input = [9, 4, 8, 3, 7, 2];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;

        var result = Indicators.Alma(input, 3, 3, 0.5, (source, output) =>
        {
            callbackInput = (double[])source.Clone();
            callbackOutput = (double[])output.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackInput);
        Assert.NotNull(callbackOutput);
        Assert.Equal(input, callbackInput);
        Assert.Equal(result.Ma, callbackOutput);
        Assert.True(double.IsFinite(callbackOutput[0]));
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Alma_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        double[] input = [double.NaN, 4, 8, 3, 7, 2];
        var invoked = false;

        var result = Indicators.Alma(input, 3, 6, 0.85, (source, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.False(result.Ma.HasValidData());
        Assert.True(result.Ma.CountValidData() > 0);
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Theory]
    [InlineData(0, 6d, 0.85)]
    [InlineData(-1, 6d, 0.85)]
    [InlineData(100001, 6d, 0.85)]
    [InlineData(3, 0d, 0.85)]
    [InlineData(3, 0.009, 0.85)]
    [InlineData(3, 6d, -0.01)]
    [InlineData(3, 6d, 1.01)]
    public void Alma_WhenParametersAreInvalid_ReturnsFailure(int period, double sigma, double offset)
    {
        var result = Indicators.Alma([5, 4, 3, 2, 1], period, sigma, offset);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Fact]
    public void Alma_WhenInputIsNull_ReturnsFailure()
    {
        var result = Indicators.Alma((double[])null!, period: 3, sigma: 6, offset: 0.85);

        Assert.False(result.Success);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
        Assert.False(string.IsNullOrWhiteSpace(result.Message));
    }

    [Theory]
    [InlineData(IndicatorCalculationBase.Close, 10d)]
    [InlineData(IndicatorCalculationBase.Open, 20d)]
    [InlineData(IndicatorCalculationBase.High, 30d)]
    [InlineData(IndicatorCalculationBase.Low, 5d)]
    [InlineData(IndicatorCalculationBase.Volume, 50d)]
    public void Alma_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double latest)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 5, Volume = 50 },
            new() { Close = 9, Open = 19, High = 29, Low = 4, Volume = 49 }
        };

        var result = Indicators.Alma(candles, 1, 6, 0.85, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal([latest, latest - 1], result.Ma);
    }

    [Fact]
    public void Alma_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20 },
            new() { Close = 9, Open = 19 }
        };
        double[]? callbackInput = null;

        var defaultResult = Indicators.Alma(candles, 1, 6, 0.85);
        var customResult = Indicators.Alma(candles, 1, 6, 0.85, IndicatorCalculationBase.Open, (input, output) =>
        {
            callbackInput = (double[])input.Clone();
            return IndicatorSignal.Buy;
        });

        Assert.True(defaultResult.Success, defaultResult.Message);
        Assert.Equal([10d, 9d], defaultResult.Ma);
        Assert.True(customResult.Success, customResult.Message);
        Assert.NotNull(callbackInput);
        Assert.Equal([20d, 19d], callbackInput);
        Assert.Equal(IndicatorSignal.Buy, customResult.IndicatorSignal);
    }
}
