using TANet.Contracts.Enums;
using TANet.Contracts.Models;
using TANet.Core;
using Xunit;

namespace TANet.Tests;

public class StochasticCompatibilityTests
{
    [Fact]
    public void LegacyOverloads_MatchCorrespondingStochasticOverloads()
    {
        double[] close = [13, 10, 12, 9, 8, 11, 7, 6, 5, 4];
        var high = close.Select(value => value + 2).ToArray();
        var low = close.Select(value => value - 2).ToArray();
        var candles = close.Select((value, index) => new Candle
        {
            High = high[index],
            Low = low[index],
            Close = value
        }).ToList();

        const int fastPeriod = 3;
        const int slowKPeriod = 2;
        const int slowDPeriod = 3;
        const MovingAverageType slowKType = MovingAverageType.Ema;
        const MovingAverageType slowDType = MovingAverageType.Wma;
        var callbackCount = 0;
        IndicatorSignal CustomSignal(double[] slowK, double[] slowD)
        {
            callbackCount++;
            return IndicatorSignal.Sell;
        }

#pragma warning disable CS0618 // Intentionally exercising the public API from version 3.0.
        var results = new[]
        {
            (Indicators.Stochastic(candles, fastPeriod, slowKPeriod, slowDPeriod),
             Indicators.Stochsatic(candles, fastPeriod, slowKPeriod, slowDPeriod)),
            (Indicators.Stochastic(candles, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod),
             Indicators.Stochsatic(candles, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod)),
            (Indicators.Stochastic(candles, fastPeriod, slowKPeriod, slowDPeriod, CustomSignal),
             Indicators.Stochsatic(candles, fastPeriod, slowKPeriod, slowDPeriod, CustomSignal)),
            (Indicators.Stochastic(candles, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod, CustomSignal),
             Indicators.Stochsatic(candles, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod, CustomSignal)),
            (Indicators.Stochastic(high, low, close, fastPeriod, slowKPeriod, slowDPeriod),
             Indicators.Stochsatic(high, low, close, fastPeriod, slowKPeriod, slowDPeriod)),
            (Indicators.Stochastic(high, low, close, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod),
             Indicators.Stochsatic(high, low, close, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod)),
            (Indicators.Stochastic(high, low, close, fastPeriod, slowKPeriod, slowDPeriod, CustomSignal),
             Indicators.Stochsatic(high, low, close, fastPeriod, slowKPeriod, slowDPeriod, CustomSignal)),
            (Indicators.Stochastic(high, low, close, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod, CustomSignal),
             Indicators.Stochsatic(high, low, close, fastPeriod, slowKType, slowKPeriod, slowDType, slowDPeriod, CustomSignal))
        };
#pragma warning restore CS0618

        foreach (var (expected, actual) in results)
        {
            Assert.True(expected.Success, expected.Message);
            Assert.True(actual.Success, actual.Message);
            Assert.Equal(expected.SlowK, actual.SlowK);
            Assert.Equal(expected.SlowD, actual.SlowD);
            Assert.Equal(expected.IndicatorSignal, actual.IndicatorSignal);
        }

        Assert.Equal(8, callbackCount);
    }
}
