# TechnicalAnalysis.NET

TechnicalAnalysis.NET provides a C# API for technical indicators calculated by
[TA-Lib](https://ta-lib.org/). It accepts price arrays or candle data and can
produce a signal using either the indicator's default logic or your own.

This branch contains the next major version, 3.0.0, with breaking API and data
ordering changes. See [Migrating to 3.0](#migrating-to-30) before upgrading.

## Requirements and installation

The main `TechnicalAnalysis.Net` package targets .NET 10. Its separate
`TechnicalAnalysis.Net.Contracts` project targets .NET Standard 2.0.

Once version 3.0.0 is published to NuGet, install it with:

```bash
dotnet add package TechnicalAnalysis.Net --version 3.0.0
```

Until then, build and reference the projects in this repository to use the
version described here. Merely merging this branch does not publish a package.

## Data ordering and results

- Provide input arrays and candle lists in **newest-first** order: index `0` is
  the latest observation, index `1` is the previous observation, and so on.
  For indicators with multiple input arrays, the arrays must have the same
  length and values at the same index must describe the same observation.
- On a successful calculation, each output series has the same length and
  ordering as the input. Positions without enough history are `double.NaN` at
  the end of the series. Inputs are not reordered in place.
- `Success` reports whether the calculation completed. A valid request with
  insufficient nonempty history can succeed with no valid output yet. On a
  failed calculation, inspect `Message` before accessing output series.
- `series.HasValidData()` checks that index `0` exists and contains a finite
  number. `series.CountValidData()` counts finite values in the whole series;
  it does not guarantee that they are consecutive. These extension methods
  require `using TANet.Util.Extensions;`.
- Check an older value with `double.IsFinite(series[index])` after checking
  that the index is within the array. A valid value at index `0` does not imply
  that every older value is valid.

## Example: simple moving average

```csharp
using System;
using TANet.Core;
using TANet.Util.Extensions;

double[] close = [15, 14, 13, 12, 11]; // newest first
var result = Indicators.Sma(close, period: 3);

if (!result.Success)
{
    Console.WriteLine(result.Message);
}
else if (result.Ma.HasValidData())
{
    Console.WriteLine($"Latest SMA: {result.Ma[0]}"); // 14
    Console.WriteLine($"Signal: {result.IndicatorSignal}");
    Console.WriteLine($"Finite values: {result.Ma.CountValidData()}"); // 3

    if (result.Ma.Length > 1 && double.IsFinite(result.Ma[1]))
        Console.WriteLine($"Previous SMA: {result.Ma[1]}"); // 13
}
else
{
    Console.WriteLine("No finite value for the latest observation yet.");
}
```

The resulting series is `[14, 13, 12, NaN, NaN]` in newest-first order.

## Custom signals

Many indicators accept a `signalLogic` callback. For example, SMA receives the
original input and the output series, both newest first:

```csharp
using TANet.Contracts.Enums;
using TANet.Core;

double[] close = [15, 14, 13, 12, 11];
var result = Indicators.Sma(close, 3, (input, output) =>
    input[0] > output[0] ? IndicatorSignal.Buy :
    input[0] < output[0] ? IndicatorSignal.Sell :
    IndicatorSignal.Stay);
```

The callback is invoked only when the output needed for the current signal is
finite. With insufficient history, the result can still succeed, but the signal
is `Stay` and the callback is not called. Default signals vary by indicator;
ATR and Bollinger Bands return `Stay` unless custom logic is provided. Signals
are simple rules, not a substitute for an evaluated trading strategy.

## Candle input

Indicators that need multiple price series can also accept a `List<Candle>`:

```csharp
using System.Collections.Generic;
using TANet.Contracts.Models;
using TANet.Core;

var candles = new List<Candle> // newest first
{
    new() { High = 14, Low = 12, Close = 13 },
    new() { High = 15, Low = 13, Close = 14 },
    new() { High = 12, Low = 10, Close = 11 },
    new() { High = 13, Low = 11, Close = 12 },
    new() { High = 11, Low = 9, Close = 10 }
};

var result = Indicators.Cci(candles, period: 3);
```

Check `result.Success` and the relevant output series as in the SMA example.

## Implemented indicators

- [x] Aroon
- [x] Aroon Oscillator
- [x] ATR
- [x] Bollinger Bands
- [x] CCI
- [x] EMA
- [x] Extended MACD
- [x] HMA — Hull Moving Average
- [x] KAMA
- [x] MACD
- [x] MFI
- [x] Parabolic SAR
- [x] RSI
- [x] SMA
- [x] Stochastic
- [x] T3 — Triple Exponential Moving Average (Tillson's T3)
- [x] TEMA
- [x] Williams %R
- [x] WMA

---

- [ ] AC — Accelerator/Decelerator Oscillator
- [ ] ACCBANDS — Acceleration Bands
- [ ] AD — Chaikin A/D Line
- [ ] ADOSC — Chaikin A/D Oscillator
- [ ] ADR — Average Day Range
- [ ] ADX — Average Directional Movement Index
- [ ] ADXR — Average Directional Movement Index Rating
- [ ] ALMA — Arnaud Legoux Moving Average
- [ ] AO — Awesome Oscillator
- [ ] APO — Absolute Price Oscillator
- [ ] ASI — Wilder Accumulative Swing Index
- [ ] AVGDEV — Average Deviation
- [ ] AVGPRICE — Average Price
- [ ] BBW — Bollinger BandWidth
- [ ] BETA — Beta
- [ ] BOP — Balance Of Power
- [ ] CE — Chandelier Exit
- [ ] CG — Center of Gravity Oscillator
- [ ] CHOP — Choppiness Index
- [ ] CHOPTR — Choppiness Index (True Range Box)
- [ ] CKSP — Chande Kroll Stop
- [ ] CMF — Chaikin Money Flow
- [ ] CMO — Chande Momentum Oscillator
- [ ] CMOU — Chande Momentum Oscillator (Unsmoothed)
- [ ] COPPOCK — Coppock Curve
- [ ] CORREL — Pearson's Correlation Coefficient (r)
- [ ] CRSI — Connors Relative Strength Index
- [ ] CTI — Correlation Trend Indicator
- [ ] CVI — Chaikin's Volatility
- [ ] DEMA — Double Exponential Moving Average
- [ ] DONCHIAN — Donchian Channels
- [ ] DOSC — Derivative Oscillator
- [ ] DPO — Detrended Price Oscillator
- [ ] DX — Directional Movement Index
- [ ] EFI — Elder's Force Index
- [ ] EMV — Arms Ease of Movement
- [ ] ER — Kaufman Efficiency Ratio
- [ ] ERI — Elder Ray Index (Bull Power / Bear Power)
- [ ] FISHER — Fisher Transform
- [ ] FOSC — Forecast Oscillator
- [ ] FRACTAL — Williams Fractal
- [ ] FRAMA — Fractal Adaptive Moving Average
- [ ] HA — Heikin-Ashi Candles
- [ ] HT_DCPERIOD — Hilbert Transform - Dominant Cycle Period
- [ ] HT_DCPHASE — Hilbert Transform - Dominant Cycle Phase
- [ ] HT_PHASOR — Hilbert Transform - Phasor Components
- [ ] HT_SINE — Hilbert Transform - SineWave
- [ ] HT_TRENDLINE — Hilbert Transform - Instantaneous Trendline
- [ ] HT_TRENDMODE — Hilbert Transform - Trend vs Cycle Mode
- [ ] IBS — Internal Bar Strength
- [ ] ICHIMOKU — Ichimoku Kinko Hyo
- [ ] IMI — Intraday Momentum Index
- [ ] KC — Keltner Channels
- [ ] KDJ — KDJ Stochastic
- [ ] KST — Know Sure Thing (Pring)
- [ ] KSTEXT — Know Sure Thing with controllable MA type
- [ ] KURTOSIS — Rolling Excess Kurtosis
- [ ] KVO — Klinger Volume Oscillator
- [ ] LINEARREG — Linear Regression
- [ ] LINEARREG_ANGLE — Linear Regression Angle
- [ ] LINEARREG_INTERCEPT — Linear Regression Intercept
- [ ] LINEARREG_SLOPE — Linear Regression Slope
- [ ] MA — Moving average (partially supported)
- [ ] MACDFIX — Moving Average Convergence/Divergence Fix 12/26
- [ ] MACDV — MACD-V, Volatility Normalised Momentum
- [ ] MAMA — MESA Adaptive Moving Average
- [ ] MARKETFI — Market Facilitation Index
- [ ] MASSI — Mass Index
- [ ] MAVP — Moving average with variable period
- [ ] MCGD — McGinley Dynamic
- [ ] MEDIAN — Rolling Median
- [ ] MEDPRICE — Median Price
- [ ] MIDPOINT — MidPoint over period
- [ ] MIDPRICE — Midpoint Price over period
- [ ] MINUS_DI — Minus Directional Indicator
- [ ] MINUS_DM — Minus Directional Movement
- [ ] MOM — Momentum
- [ ] NATR — Normalized Average True Range
- [ ] NVI — Negative Volume Index
- [ ] OBV — On Balance Volume
- [ ] PERCENTB — Bollinger Bands %B
- [ ] PERCENTILE — Percentile (nearest rank)
- [ ] PERCENTRANK — Percent Rank
- [ ] PLUS_DI — Plus Directional Indicator
- [ ] PLUS_DM — Plus Directional Movement
- [ ] PPO — Percentage Price Oscillator
- [ ] PSO — Premier Stochastic Oscillator
- [ ] PVI — Positive Volume Index
- [ ] PVO — Percentage Volume Oscillator
- [ ] PVT — Price Volume Trend
- [ ] QSTICK — Qstick
- [ ] RMA — Wilder's Smoothed Moving Average
- [ ] ROC — Rate of change : ((price/prevPrice)-1)*100
- [ ] ROCP — Rate of change Percentage: (price-prevPrice)/prevPrice
- [ ] ROCR — Rate of change ratio: (price/prevPrice)
- [ ] ROCR100 — Rate of change ratio 100 scale: (price/prevPrice)*100
- [ ] ROGERSSATCHELL — Rogers-Satchell Volatility
- [ ] RVI — Relative Volatility Index
- [ ] RVIR — Relative Volatility Index, refined high/low form
- [ ] RVOL — Relative Volume
- [ ] SAREXT — Parabolic SAR - Extended
- [ ] SI — Wilder Swing Index
- [ ] SKEW — Rolling Skewness
- [ ] SMI — Stochastic Momentum Index
- [ ] SQZMOM — Squeeze Momentum and Level
- [ ] STC — Schaff Trend Cycle
- [ ] STDDEV — Standard Deviation
- [ ] STOCHF — Stochastic Fast
- [ ] STOCHRSI — Stochastic Relative Strength Index
- [ ] SUPERTREND — SuperTrend
- [ ] SWAK_2PHP — Swiss Army Knife - Two-Pole High-Pass Filter
- [ ] SWAK_BP — Swiss Army Knife - Band-Pass Filter
- [ ] SWAK_BUTTER — Swiss Army Knife - Butterworth Filter
- [ ] SWAK_GAUSS — Swiss Army Knife - Gaussian Filter
- [ ] SWAK_HP — Swiss Army Knife - High-Pass Filter
- [ ] TRANGE — True Range
- [ ] TRIMA — Triangular Moving Average
- [ ] TRIX — 1-day Rate-Of-Change (ROC) of a Triple Smooth EMA
- [ ] TSF — Time Series Forecast
- [ ] TSI — True Strength Index
- [ ] TYPPRICE — Typical Price
- [ ] ULTOSC — Ultimate Oscillator
- [ ] VAR — Variance
- [ ] VHF — Vertical Horizontal Filter
- [ ] VIDYA — Variable Index Dynamic Average
- [ ] VORTEX — Vortex Indicator
- [ ] VWAP — Volume Weighted Average Price
- [ ] VWMA — Volume Weighted Moving Average
- [ ] WAD — Williams' Accumulation/Distribution
- [ ] WAVETREND — WaveTrend Oscillator
- [ ] WCLPRICE — Weighted Close Price
- [ ] ZIGZAG — Zig Zag
- [ ] ZLEMA — Zero-Lag Exponential Moving Average

The list above follows the [TA-Lib function reference](https://ta-lib.org/functions/).
`MA` is partially supported through the existing moving average methods; not all
reference MA types are exposed. `CE` and `MACDV` appear in the current reference
but not in the installed TA-Lib C# 0.8.3 API, so they require a dependency review.

The array-based Williams %R overload currently requires a `volume` parameter
for compatibility, although the calculation does not use it. TA-Lib's other
technical indicators are planned; candlestick patterns and mathematical
functions are outside the current scope.

## Migrating to 3.0

- Use `double` and `double[]` for price data and indicator outputs instead of
  `decimal` and `decimal[]`.
- Supply observations newest first and read the latest output at index `0`.
  Outputs retain the input length, with `NaN` for unavailable history.
- Check `Success` separately from `HasValidData()`: a successful calculation
  does not necessarily mean that the latest value is available yet. Update
  custom signal logic to use the newest-first series and to check any older
  values it accesses.
- The main package now targets .NET 10 and uses the official C# TA-Lib package.
  The legacy `Cryptowatcher.TA-LIb-Core` dependency and the public
  `MovingAverageTypes.FromTaLib` and `MovingAverageTypes.ToTaLib` methods that
  used its types have been removed. `ToOfficialTaLib` converts TANet's supported
  moving average types to the official `TALib.MAType`.

## Build and test

Install the .NET 10 SDK, then run from the repository root:

```bash
dotnet restore TANet/TANet.sln
dotnet test TANet/TANet.sln --configuration Release
```

## Contributing

Issues and pull requests are welcome. Describe the problem or proposed change,
and include tests for behavioral changes. For indicator additions, follow the
newest-first and warm-up contract above and compare calculations against the
official TA-Lib implementation. Run the test suite before submitting a pull
request.

## License

See [LICENSE](LICENSE).
