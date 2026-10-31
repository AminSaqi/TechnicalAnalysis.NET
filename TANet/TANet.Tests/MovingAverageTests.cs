
using TANet.Contracts.Enums;
using TANet.Core;
using Xunit;


namespace TANet.Tests;


public class SmaTests
{
    [Fact]
    public void Sma_ReturnsValuesAfterWarmup()
    {
        var result = Indicators.Sma(
            [1, 2, 3, 4, 5],
            period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected: [ 2, 3, 4 ], actual: result.Ma);
        Assert.Equal(expected: IndicatorSignal.Buy, actual: result.IndicatorSignal);
    }

    [Fact]
    public void Sma_WhenHistoryIsTooShort_ReturnsFailure()
    {
        var result = Indicators.Sma(
            [1, 2],
            period: 3);

        Assert.False(result.Success);
        Assert.Equal(expected: IndicatorSignal.Stay, actual: result.IndicatorSignal);
        Assert.NotEmpty(result.Message);
    }
}


public class MovingAverageParityTests
{
    [Theory]
    [InlineData(MovingAverageType.Sma, TALib.MAType.SMA)]
    [InlineData(MovingAverageType.Wma, TALib.MAType.WMA)]
    [InlineData(MovingAverageType.Ema, TALib.MAType.EMA)]
    [InlineData(MovingAverageType.Kama, TALib.MAType.KAMA)]
    [InlineData(MovingAverageType.Tema, TALib.MAType.TEMA)]
    public void Ma_MatchesOfficialTaLibOutput(
        MovingAverageType maType,
        TALib.MAType officialMaType)
    {
        decimal[] input =
        [
            10m, 11m, 13m, 12m, 15m, 14m, 18m, 17m,
        20m, 19m, 23m, 22m, 25m, 24m, 28m, 27m
        ];

        const int period = 3;

        var tanetResult = maType switch
        {
            MovingAverageType.Sma => Indicators.Sma(input, period),
            MovingAverageType.Wma => Indicators.Wma(input, period),
            MovingAverageType.Ema => Indicators.Ema(input, period),
            MovingAverageType.Kama => Indicators.Kama(input, period),
            MovingAverageType.Tema => Indicators.Tema(input, period),
            _ => throw new ArgumentOutOfRangeException(nameof(maType))
        };

        Assert.True(tanetResult.Success, tanetResult.Message);

        var inputDouble = Array.ConvertAll(input, value => (double)value);
        var officialOutput = new double[input.Length];

        var range = TALib.Core.Default.Ma(
            0,
            inputDouble.Length - 1,
            inputDouble,
            period,
            officialMaType,
            officialOutput);

        Assert.True(range.Count > 0);

        var expected = new decimal[range.Count];
        for (var i = 0; i < range.Count; i++)
        {
            expected[i] = (decimal)officialOutput[i];
        }

        Assert.Equal(expected: range.Count, actual: tanetResult.Ma.Length);
        Assert.Equal(expected: expected, actual: tanetResult.Ma);
    }
}
