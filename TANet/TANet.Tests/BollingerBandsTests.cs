using TANet.Contracts.Enums;
using TANet.Core;
using TANet.Util.Extensions;


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
        const double standardDeviationDown = 1.5;

        var result = Indicators.BollingerBands(
            input,
            period,
            standardDeviationUp,
            standardDeviationDown,
            maType);

        Assert.True(result.Success, result.Message);

        var chronologicalInput = input.Reverse().ToArray();
        var expectedUpper = new double[input.Length];
        var expectedMiddle = new double[input.Length];
        var expectedLower = new double[input.Length];

        var range = TALib.Core.Default.Bbands(
            0,
            chronologicalInput.Length - 1,
            chronologicalInput,
            period,
            standardDeviationUp,
            standardDeviationDown,
            officialMaType,
            expectedUpper,
            expectedMiddle,
            expectedLower);

        Assert.True(range.Count > 0);

        void AssertBand(double[] expected, double[] actual)
        {
            Assert.Equal(input.Length, actual.Length);

            Assert.Equal(
                expected.Take(range.Count).Reverse(),
                actual.Take(range.Count));

            Assert.All(
                actual.Skip(range.Count),
                value => Assert.True(double.IsNaN(value)));

            Assert.True(actual.HasValidData());
            Assert.Equal(range.Count, actual.CountValidData());
        }

        AssertBand(expectedUpper, result.UpperBand);
        AssertBand(expectedMiddle, result.MiddleBand);
        AssertBand(expectedLower, result.LowerBand);

        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Bbands_WhenHistoryIsTooShort_ReturnsNaNValues()
    {
        var result = Indicators.BollingerBands(
            [1, 2],
            period: 3,
            standardDeviationUp: 2,
            standardDeviationDown: 2);

        Assert.True(result.Success, result.Message);

        void AssertBand(double[] band)
        {
            Assert.Equal(2, band.Length);
            Assert.All(band, value => Assert.True(double.IsNaN(value)));
            Assert.False(band.HasValidData());
            Assert.Equal(0, band.CountValidData());
        }

        AssertBand(result.UpperBand);
        AssertBand(result.MiddleBand);
        AssertBand(result.LowerBand);

        Assert.Equal(expected: IndicatorSignal.Stay, actual: result.IndicatorSignal);       
    }
}

