using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using TANet.Util.Extensions;
using TANet.Util.StaticClasses.EnumConvertors;
using Xunit;

namespace TANet.Tests;

public class GenericMovingAverageTests
{
    public static TheoryData<MovingAverageType, TALib.MAType> MaTypes => new()
    {
        { MovingAverageType.Sma, TALib.MAType.SMA },
        { MovingAverageType.Wma, TALib.MAType.WMA },
        { MovingAverageType.Ema, TALib.MAType.EMA },
        { MovingAverageType.Kama, TALib.MAType.KAMA },
        { MovingAverageType.Tema, TALib.MAType.TEMA },
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
        Enumerable.Range(1, 128)
            .Select(value => 100d + value * 0.2 + Math.Sin(value * 0.3) * 5)
            .Reverse()
            .ToArray();

    [Theory]
    [MemberData(nameof(MaTypes))]
    public void Ma_AllTypesMatchOfficialTaLib(MovingAverageType type, TALib.MAType officialType)
    {
        var input = SampleInput();
        var original = (double[])input.Clone();
        const int period = 5;
        var result = Indicators.Ma(input, type, period);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Ma(0, input.Length - 1, input.Reverse().ToArray(), period, officialType, expected);

        Assert.Equal(officialType, MovingAverageTypes.ToOfficialTaLib(type));
        Assert.True(result.Success, result.Message);
        Assert.True(range.Count > 0);
        Assert.Equal(input.Length, result.Ma.Length);
        Assert.Equal(expected.Take(range.Count).Reverse(), result.Ma.Take(range.Count));
        Assert.All(result.Ma.Skip(range.Count), value => Assert.True(double.IsNaN(value)));
        Assert.True(result.Ma.HasValidData());
        Assert.Equal(range.Count, result.Ma.CountValidData());
        Assert.Equal(original, input);
    }

    [Theory]
    [MemberData(nameof(MaTypes))]
    public void Ma_AllTypesReturnInputForPeriodOne(MovingAverageType type, TALib.MAType officialType)
    {
        double[] input = [5, 4, 3, 2, 1];
        var result = Indicators.Ma(input, type, period: 1);
        var expected = new double[input.Length];
        var range = TALib.Core.Default.Ma(0, input.Length - 1, input.Reverse().ToArray(), 1, officialType, expected);

        Assert.True(result.Success, result.Message);
        Assert.Equal(input.Length, range.Count);
        Assert.Equal(expected.Reverse(), result.Ma);
        Assert.Equal(input, result.Ma);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void MaType_PreservesPublishedNumericValuesAndCoversReferenceTypes()
    {
        Assert.Equal(0, (int)MovingAverageType.Sma);
        Assert.Equal(1, (int)MovingAverageType.Wma);
        Assert.Equal(2, (int)MovingAverageType.Ema);
        Assert.Equal(3, (int)MovingAverageType.Kama);
        Assert.Equal(4, (int)MovingAverageType.Tema);

        var mapped = Enum.GetValues<MovingAverageType>()
            .Select(MovingAverageTypes.ToOfficialTaLib)
            .OrderBy(value => (int)value);
        var official = Enum.GetValues<TALib.MAType>().OrderBy(value => (int)value);
        Assert.Equal(official, mapped);
    }

    [Fact]
    public void Ma_UnknownTypeRetainsExistingSmaFallback()
    {
        double[] input = [5, 4, 3, 2, 1];
        var result = Indicators.Ma(input, (MovingAverageType)999, period: 3);
        var expected = Indicators.Sma(input, period: 3);

        Assert.True(result.Success, result.Message);
        Assert.True(expected.Success, expected.Message);
        Assert.Equal(expected.Ma, result.Ma);
    }

    [Fact]
    public void Ma_DefaultAndDisabledFollowReferenceSemantics()
    {
        double[] input = [5, 4, 3, 2, 1];
        var defaultResult = Indicators.Ma(input, MovingAverageType.Default, period: 3);
        var sma = Indicators.Sma(input, period: 3);
        var disabled = Indicators.Ma(input, MovingAverageType.Disabled, period: 3);

        Assert.True(defaultResult.Success, defaultResult.Message);
        Assert.True(sma.Success, sma.Message);
        Assert.True(disabled.Success, disabled.Message);
        Assert.Equal(sma.Ma, defaultResult.Ma);
        Assert.Equal(input, disabled.Ma);
    }

    [Fact]
    public void Ma_SpecializedTypesUseTheReferenceDefaults()
    {
        var input = SampleInput();
        const int period = 5;
        var alma = Indicators.Alma(input, period, sigma: 6, offset: 0.85);
        var t3 = Indicators.T3(input, period, vFactor: 0.7);
        var mama = Indicators.Mama(input, fastLimit: 0.5, slowLimit: 0.05);
        var vidya = Indicators.Vidya(input, period, cmoPeriod: (3 * period + 2) / 4);

        void AssertSame(double[] expected, MovingAverageType type)
        {
            var actual = Indicators.Ma(input, type, period);
            Assert.True(actual.Success, actual.Message);
            Assert.Equal(expected, actual.Ma);
        }

        Assert.True(alma.Success, alma.Message);
        Assert.True(t3.Success, t3.Message);
        Assert.True(mama.Success, mama.Message);
        Assert.True(vidya.Success, vidya.Message);
        AssertSame(alma.Ma, MovingAverageType.Alma);
        AssertSame(t3.Ma, MovingAverageType.T3);
        AssertSame(mama.Mama, MovingAverageType.Mama);
        AssertSame(vidya.Ma, MovingAverageType.Vidya);
    }

    [Fact]
    public void Ma_MamaPeriodOneCopiesInputAndLargerPeriodsUseTheSameMama()
    {
        var input = SampleInput();
        var one = Indicators.Ma(input, MovingAverageType.Mama, period: 1);
        var three = Indicators.Ma(input, MovingAverageType.Mama, period: 3);
        var ten = Indicators.Ma(input, MovingAverageType.Mama, period: 10);

        Assert.True(one.Success, one.Message);
        Assert.True(three.Success, three.Message);
        Assert.True(ten.Success, ten.Message);
        Assert.Equal(input, one.Ma);
        Assert.Equal(three.Ma, ten.Ma);
    }

    [Fact]
    public void Ma_WhenHistoryIsTooShort_SucceedsWithoutCallingSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Ma([2, 1], MovingAverageType.Dema, 3, (input, output) =>
        {
            invoked = true;
            return IndicatorSignal.Buy;
        });

        Assert.True(result.Success, result.Message);
        Assert.Equal(2, result.Ma.Length);
        Assert.All(result.Ma, value => Assert.True(double.IsNaN(value)));
        Assert.False(result.Ma.HasValidData());
        Assert.False(invoked);
        Assert.Equal(IndicatorSignal.Stay, result.IndicatorSignal);
    }

    [Fact]
    public void Ma_PassesNewestFirstInputAndOutputToCustomSignalLogic()
    {
        double[] input = [5, 4, 3, 2, 1];
        double[]? callbackInput = null;
        double[]? callbackOutput = null;
        var result = Indicators.Ma(input, MovingAverageType.Sma, 3, (source, output) =>
        {
            callbackInput = (double[])source.Clone();
            callbackOutput = (double[])output.Clone();
            return IndicatorSignal.Sell;
        });

        Assert.True(result.Success, result.Message);
        Assert.NotNull(callbackInput);
        Assert.NotNull(callbackOutput);
        Assert.Equal(input, callbackInput);
        Assert.Equal([4d, 3d, 2d, double.NaN, double.NaN], callbackOutput);
        Assert.Equal(result.Ma, callbackOutput);
        Assert.Equal(IndicatorSignal.Sell, result.IndicatorSignal);
    }

    [Fact]
    public void Ma_WhenCurrentValueIsNotFinite_DoesNotCallSignalLogic()
    {
        var invoked = false;
        var result = Indicators.Ma([double.NaN, 4, 3, 2, 1], MovingAverageType.Sma, 3, (input, output) =>
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

    [Fact]
    public void Ma_WhenPeriodIsInvalid_ReturnsFailure()
    {
        var result = Indicators.Ma([5, 4, 3, 2, 1], MovingAverageType.Sma, period: 0);

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
    public void Ma_FromCandles_UsesSelectedSeries(IndicatorCalculationBase calculationBase, double latest)
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20, High = 30, Low = 5, Volume = 50 },
            new() { Close = 9, Open = 19, High = 29, Low = 4, Volume = 49 }
        };

        var result = Indicators.Ma(candles, MovingAverageType.Hma, 1, calculationBase);

        Assert.True(result.Success, result.Message);
        Assert.Equal([latest, latest - 1], result.Ma);
    }

    [Fact]
    public void Ma_FromCandles_DefaultsToCloseAndPassesSelectedSeriesToSignalLogic()
    {
        var candles = new List<Candle>
        {
            new() { Close = 10, Open = 20 },
            new() { Close = 9, Open = 19 }
        };
        double[]? callbackInput = null;
        var defaultResult = Indicators.Ma(candles, MovingAverageType.Hma, 1);
        var customResult = Indicators.Ma(candles, MovingAverageType.Hma, 1, IndicatorCalculationBase.Open, (input, output) =>
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
