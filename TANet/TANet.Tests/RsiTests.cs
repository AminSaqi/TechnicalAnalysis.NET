
using TANet.Contracts.Enums;
using TANet.Core;
using TANet.Util.Extensions;
using Xunit;


namespace TANet.Tests;


public class RsiTests
{
    [Fact]
    public void Rsi_ReturnsNewestFirstValuesMatchingOfficialTaLib()
    {
        double[] input =
        [
            27, 28, 24, 25, 22, 23, 19, 20,
            17, 18, 14, 15, 12, 13, 11, 10
        ];

        const int period = 3;

        var result = Indicators.Rsi(
            input,
            period);

        Assert.True(result.Success, result.Message);

        var chronologicalInput = input.Reverse().ToArray();
        var officialOutput = new double[input.Length];

        var range = TALib.Core.Default.Rsi(
            0,
            chronologicalInput.Length - 1,
            chronologicalInput,
            period,
            officialOutput);

        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Rsi.Length);

        var expectedValues = officialOutput.Take(range.Count).Reverse().ToArray();

        Assert.Equal(expected: expectedValues, actual: result.Rsi[..range.Count]);

        Assert.All(
            result.Rsi[range.Count..],
            value => Assert.True(double.IsNaN(value)));

        Assert.True(result.Rsi.HasValidData());
        Assert.Equal(expected: range.Count, actual: result.Rsi.CountValidData());
    }

    [Fact]
    public void Rsi_WhenHistoryIsTooShort_ReturnsNaNValues()
    {
        var result = Indicators.Rsi(
            [2, 1],
            period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Rsi.Length);
        Assert.All(result.Rsi, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Rsi.HasValidData());
        Assert.Equal(0, result.Rsi.CountValidData());
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Rsi_CheckDefaultSignal_WhenLatestValueIsOS_ReturnsBuy()
    {     
        var result = Indicators.Rsi([1, 2, 3, 4], period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Buy, result.IndicatorSignal);
    }

    [Fact]
    public void Rsi_CheckdefaultSignal_WhenLatestValueIsOB_ReturnsSell()
    {        
        var result = Indicators.Rsi([4, 3, 2, 1], period: 3);

        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Rsi_CheckdefaultSignal_WhenLatestValueIsNeutral_ReturnsStay()
    {
        var result = Indicators.Rsi([1, 2, 1, 2, 1], period: 4);

        Assert.True(result.Success, result.Message);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }
}



