
using TANet.Contracts.Enums;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;


namespace TANet.Tests;


public class SmaTests
{
    [Fact]
    public void Sma_ReturnsValuesAfterWarmup()
    {
        var result = Indicators.Sma(
            [5, 4, 3, 2, 1],
            period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(expected: [4, 3, 2, double.NaN, double.NaN], actual: result.Ma);
        Assert.Equal(expected: IndicatorSignal.Buy, actual: result.IndicatorSignal);
    }

    [Fact]
    public void Sma_WhenHistoryIsTooShort_ReturnsNaNValues()
    {
        var result = Indicators.Sma(
            [2, 1],
            period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.Equal(0, result.Ma.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
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

        var chronologicalInput = input.Reverse().ToArray();
        var officialOutput = new double[input.Length];

        var range = TALib.Core.Default.Ma(
            0,
            chronologicalInput.Length - 1,
            chronologicalInput,
            period,
            officialMaType,
            officialOutput);

        Assert.True(range.Count > 0);            
        Assert.Equal(expected: input.Length, actual: tanetResult.Ma.Length);

        var expectedValues = officialOutput.Take(range.Count).Reverse().ToArray();

        Assert.Equal(expected: expectedValues, actual: tanetResult.Ma[..range.Count]);

        Assert.All(
            tanetResult.Ma[range.Count..],
            value => Assert.True(double.IsNaN(value)));

        Assert.True(tanetResult.Ma.HasValidData());
        Assert.Equal(expected: range.Count, actual: tanetResult.Ma.CountValidData());
    }
}
