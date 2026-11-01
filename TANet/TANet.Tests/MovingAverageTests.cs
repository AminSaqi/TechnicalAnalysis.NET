
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
        double[] input =
        [
            10, 11, 13, 12, 15, 14, 18, 17,
            20, 19, 23, 22, 25, 24, 28, 27
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

        var officialOutput = new double[input.Length];

        var range = TALib.Core.Default.Ma(
            0,
            input.Length - 1,
            input,
            period,
            officialMaType,
            officialOutput);

        Assert.True(range.Count > 0);       
        Assert.Equal(expected: range.Count, actual: tanetResult.Ma.Length);
        Assert.Equal(expected: officialOutput[..range.Count], actual: tanetResult.Ma);
    }
}
