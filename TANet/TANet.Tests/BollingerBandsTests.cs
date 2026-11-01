using TANet.Contracts.Enums;
using TANet.Core;


namespace TANet.Tests;


public class BollingerBandsTests
{
    [Theory]
    [InlineData(MovingAverageType.Sma, TALib.MAType.SMA)]
    [InlineData(MovingAverageType.Wma, TALib.MAType.WMA)]
    [InlineData(MovingAverageType.Ema, TALib.MAType.EMA)]
    [InlineData(MovingAverageType.Kama, TALib.MAType.KAMA)]
    [InlineData(MovingAverageType.Tema, TALib.MAType.TEMA)]
    public void Bbands_ReturnsValuesMatchingOfficialTaLib(MovingAverageType maType, TALib.MAType officialMaType)
    {
        double[] input =
        [
            10, 11, 13, 12, 15, 14, 18, 17,
            20, 19, 23, 22, 25, 24, 28, 27
        ];

        const int period = 3;
        const double standardDeviationUp = 2;
        const double standardDeviationDown = 2;

        var result = Indicators.BollingerBands(
            input,
            period,
            standardDeviationUp,
            standardDeviationDown);

        Assert.True(result.Success, result.Message);

        var expectedUpper = new double[input.Length];
        var expectedMiddle = new double[input.Length];
        var expectedLower = new double[input.Length];

        var range = TALib.Core.Default.Bbands(
            0,
            input.Length - 1,
            input,
            period,
            standardDeviationUp,
            standardDeviationDown,
            officialMaType,
            expectedUpper,
            expectedMiddle,
            expectedLower);

        Assert.True(range.Count > 0);

        Assert.Equal(range.Count, result.UpperBand.Length);
        Assert.Equal(range.Count, result.MiddleBand.Length);
        Assert.Equal(range.Count, result.LowerBand.Length);

        Assert.Equal(expectedUpper[..range.Count], result.UpperBand);
        Assert.Equal(expectedMiddle[..range.Count], result.MiddleBand);
        Assert.Equal(expectedLower[..range.Count], result.LowerBand);

        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);

    }

    [Fact]
    public void Bbands_WhenHistoryIsTooShort_ReturnsFailure()
    {
        var result = Indicators.BollingerBands(
            [1, 2],
            period: 3,
            standardDeviationUp: 2,
            standardDeviationDown: 2);

        Assert.False(result.Success);
        Assert.Equal(expected: IndicatorSignal.Stay, actual: result.IndicatorSignal);
        Assert.NotEmpty(result.Message);
    }
}

