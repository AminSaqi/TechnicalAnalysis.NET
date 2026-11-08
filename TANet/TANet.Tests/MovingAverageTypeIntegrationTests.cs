using TANet.Contracts.Enums;
using TANet.Core;
using Xunit;

namespace TANet.Tests;

public class MovingAverageTypeIntegrationTests
{
    public static TheoryData<MovingAverageType, TALib.MAType> AddedTypes => new()
    {
        { MovingAverageType.Dema, TALib.MAType.DEMA },
        { MovingAverageType.Trima, TALib.MAType.TRIMA },
        { MovingAverageType.Mama, TALib.MAType.MAMA },
        { MovingAverageType.T3, TALib.MAType.T3 },
        { MovingAverageType.Hma, TALib.MAType.HMA },
        { MovingAverageType.Disabled, TALib.MAType.DISABLED },
        { MovingAverageType.Default, TALib.MAType.DEFAULT },
        { MovingAverageType.Zlema, TALib.MAType.ZLEMA },
        { MovingAverageType.Rma, TALib.MAType.RMA },
        { MovingAverageType.Vidya, TALib.MAType.VIDYA },
        { MovingAverageType.Alma, TALib.MAType.ALMA }
    };

    private static double[] SampleInput() =>
        Enumerable.Range(1, 200)
            .Select(value => 100d + value * 0.2 + Math.Sin(value * 0.3) * 5)
            .Reverse()
            .ToArray();

    private static void AssertSeries(double[] expected, double[] actual, int inputLength, int count)
    {
        Assert.Equal(inputLength, actual.Length);
        Assert.Equal(expected.Take(count).Reverse(), actual.Take(count));
        Assert.All(actual.Skip(count), value => Assert.True(double.IsNaN(value)));
    }

    [Theory]
    [MemberData(nameof(AddedTypes))]
    public void BollingerBands_AddedMaTypesMatchOfficialTaLib(MovingAverageType type, TALib.MAType officialType)
    {
        var input = SampleInput();
        var result = Indicators.BollingerBands(input, 5, 2, 2, type);
        var expectedUpper = new double[input.Length];
        var expectedMiddle = new double[input.Length];
        var expectedLower = new double[input.Length];
        var range = TALib.Core.Default.Bbands(
            0, input.Length - 1, input.Reverse().ToArray(), 5, 2, 2, officialType,
            expectedUpper, expectedMiddle, expectedLower);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        AssertSeries(expectedUpper, result.UpperBand, input.Length, range.Count);
        AssertSeries(expectedMiddle, result.MiddleBand, input.Length, range.Count);
        AssertSeries(expectedLower, result.LowerBand, input.Length, range.Count);
    }

    [Theory]
    [MemberData(nameof(AddedTypes))]
    public void Stochastic_AddedMaTypesMatchOfficialTaLib(MovingAverageType type, TALib.MAType officialType)
    {
        var close = SampleInput();
        var high = close.Select(value => value + 2).ToArray();
        var low = close.Select(value => value - 3).ToArray();
        var result = Indicators.Stochastic(high, low, close, 3, type, 4, type, 5);
        var expectedK = new double[close.Length];
        var expectedD = new double[close.Length];
        var range = TALib.Core.Default.Stoch(
            0, close.Length - 1,
            high.Reverse().ToArray(), low.Reverse().ToArray(), close.Reverse().ToArray(),
            3, 4, officialType, 5, officialType, expectedK, expectedD);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        AssertSeries(expectedK, result.SlowK, close.Length, range.Count);
        AssertSeries(expectedD, result.SlowD, close.Length, range.Count);
    }

    [Theory]
    [MemberData(nameof(AddedTypes))]
    public void ExtendedMacd_AddedMaTypesMatchOfficialTaLib(MovingAverageType type, TALib.MAType officialType)
    {
        var input = SampleInput();
        var result = Indicators.ExtendedMacd(
            input, type, 3, MovingAverageType.Sma, 5, type, 4);
        var expectedMacd = new double[input.Length];
        var expectedSignal = new double[input.Length];
        var expectedHistogram = new double[input.Length];
        var range = TALib.Core.Default.Macdext(
            0, input.Length - 1, input.Reverse().ToArray(),
            3, officialType, 5, TALib.MAType.SMA, 4, officialType,
            expectedMacd, expectedSignal, expectedHistogram);

        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        AssertSeries(expectedMacd, result.Macd, input.Length, range.Count);
        AssertSeries(expectedSignal, result.Signal, input.Length, range.Count);
        AssertSeries(expectedHistogram, result.Histogram, input.Length, range.Count);
    }
}
