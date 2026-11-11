# TechnicalAnalysis.NET

TechnicalAnalysis.NET provides a C# API for technical indicators calculated by
[TA-Lib](https://ta-lib.org/). It accepts price arrays or candle data and can
produce a signal using either the indicator's default logic or your own.

Version 3.0 introduced breaking API and data ordering changes. If you are
upgrading from an earlier major version, see [Migrating to 3.0](#migrating-to-30).

## Requirements and installation

The main `TechnicalAnalysis.Net` package targets .NET 10. Its separate
`TechnicalAnalysis.Net.Contracts` project targets .NET Standard 2.0.

Install version 3.1.0 with:

```bash
dotnet add package TechnicalAnalysis.Net --version 3.1.0
```

The examples and API described here target version 3.1.0.

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

- [x] ALMA — Arnaud Legoux Moving Average
- [x] Aroon
- [x] Aroon Oscillator
- [x] ATR
- [x] Bollinger Bands
- [x] CCI
- [x] DEMA — Double Exponential Moving Average
- [x] DONCHIAN — Donchian Channels
- [x] EMA
- [x] Extended MACD
- [x] HMA — Hull Moving Average
- [x] ICHIMOKU — Ichimoku Kinko Hyo
- [x] KAMA
- [x] KC — Keltner Channels
- [x] MA — Moving Average
- [x] MACD
- [x] MAMA — MESA Adaptive Moving Average
- [x] MFI
- [x] MOM — Momentum
- [x] Parabolic SAR
- [x] RMA — Wilder's Smoothed Moving Average
- [x] RSI
- [x] SMA
- [x] Stochastic
- [x] SUPERTREND — SuperTrend
- [x] T3 — Triple Exponential Moving Average (Tillson's T3)
- [x] TEMA
- [x] TRIMA — Triangular Moving Average
- [x] VIDYA — Variable Index Dynamic Average
- [x] VWAP — Volume Weighted Average Price
- [x] VWMA — Volume Weighted Moving Average
- [x] Williams %R
- [x] WMA
- [x] ZLEMA — Zero-Lag Exponential Moving Average

---

- [ ] AC — Accelerator/Decelerator Oscillator
- [ ] ACCBANDS — Acceleration Bands
- [ ] AD — Chaikin A/D Line
- [ ] ADOSC — Chaikin A/D Oscillator
- [ ] ADR — Average Day Range
- [ ] ADX — Average Directional Movement Index
- [ ] ADXR — Average Directional Movement Index Rating
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
- [ ] IMI — Intraday Momentum Index
- [ ] KDJ — KDJ Stochastic
- [ ] KST — Know Sure Thing (Pring)
- [ ] KSTEXT — Know Sure Thing with controllable MA type
- [ ] KURTOSIS — Rolling Excess Kurtosis
- [ ] KVO — Klinger Volume Oscillator
- [ ] LINEARREG — Linear Regression
- [ ] LINEARREG_ANGLE — Linear Regression Angle
- [ ] LINEARREG_INTERCEPT — Linear Regression Intercept
- [ ] LINEARREG_SLOPE — Linear Regression Slope
- [ ] MACDFIX — Moving Average Convergence/Divergence Fix 12/26
- [ ] MACDV — MACD-V, Volatility Normalised Momentum
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
- [ ] SWAK_2PHP — Swiss Army Knife - Two-Pole High-Pass Filter
- [ ] SWAK_BP — Swiss Army Knife - Band-Pass Filter
- [ ] SWAK_BUTTER — Swiss Army Knife - Butterworth Filter
- [ ] SWAK_GAUSS — Swiss Army Knife - Gaussian Filter
- [ ] SWAK_HP — Swiss Army Knife - High-Pass Filter
- [ ] TRANGE — True Range
- [ ] TRIX — 1-day Rate-Of-Change (ROC) of a Triple Smooth EMA
- [ ] TSF — Time Series Forecast
- [ ] TSI — True Strength Index
- [ ] TYPPRICE — Typical Price
- [ ] ULTOSC — Ultimate Oscillator
- [ ] VAR — Variance
- [ ] VHF — Vertical Horizontal Filter
- [ ] VORTEX — Vortex Indicator
- [ ] WAD — Williams' Accumulation/Distribution
- [ ] WAVETREND — WaveTrend Oscillator
- [ ] WCLPRICE — Weighted Close Price
- [ ] ZIGZAG — Zig Zag

The list above follows the [TA-Lib function reference](https://ta-lib.org/functions/).
`CE` and `MACDV` appear in the current reference
but not in the installed TA-Lib C# 0.8.3 API, so they require a dependency review.

The array-based Williams %R overload currently requires a `volume` parameter
for compatibility, although the calculation does not use it. TA-Lib's other
technical indicators are planned; candlestick patterns and mathematical
functions are outside the current scope.

VWAP accumulates typical price weighted by volume from the oldest supplied bar.
It does not reset at session or date boundaries, including for candle input;
pass one session at a time when session-anchored VWAP is required. TA-Lib expects
non-negative volume, skips bars with non-finite price or volume, and carries the
previous output across zero-volume bars. Before any volume has traded, it returns
zero: `HasValidData()` therefore returns true even though the weighted average is
not yet defined. The default signal is `Stay`. Custom callbacks receive high,
low, close, volume and VWAP, in that order, when the latest output is finite;
they must check volume themselves if their strategy needs traded volume.

VWMA expects non-negative volumes aligned with the price series. For periods
greater than one, an all-zero-volume window produces `NaN` even after warm-up;
at period one, TA-Lib returns the input price regardless of volume. Its custom
signal callback receives the price and VWMA output series.

MAMA returns both the MAMA and FAMA series. Its default signal compares their
latest values: MAMA above FAMA is `Buy`, below is `Sell`, and equal is `Stay`.
This compares current values and does not detect a crossover event. Custom
signal callbacks receive MAMA followed by FAMA.

Donchian Channels return the highest high, the midpoint, and the lowest low
over the period, including the current bar. The default signal is `Stay`.
Custom signal callbacks receive the high, low, upper, middle, and lower series,
in that order. A breakout strategy should compare the current price with the
previous band's value, checking that the previous value is finite first.

Ichimoku returns Tenkan-sen, Kijun-sen, Senkou Span A and Senkou Span B.
Each series becomes available as soon as its own required history exists;
this differs from the TA-Lib Ichimoku function's shared longest-period warm-up.
Values are aligned with the bars that computed them and are not shifted. To
draw the cloud, plot both Senkou spans `kijunPeriod` bars forward; for the
cloud displayed at the current bar, inspect index `kijunPeriod` after checking
that it exists and is finite. Chikou is the close plotted `kijunPeriod` bars
back and is not returned. The default signal is `Stay`; a custom callback
receives high, low, Tenkan, Kijun, Span A and Span B, in that order, once all
four current outputs are finite.

Keltner Channels use TA-Lib's variant: the middle band is an EMA of typical
price `(high + low + close) / 3`, while the upper and lower bands are the middle
band plus or minus `multiplier` times Wilder's ATR. The EMA and ATR periods are
independent. All three bands follow TA-Lib's shared KC warm-up; computing the
EMA separately may produce different values due to its initialization.
The default signal is `Stay`. Custom callbacks receive high, low, close, upper,
middle and lower series, in that order, once all current band values are finite.

SuperTrend returns the trailing line and a `Trend` series: `+1` for an uptrend
and `-1` for a downtrend. Both series use `NaN` during warm-up; `Trend` is a
`double[]` to support this convention, although TA-Lib returns integer signs.
TA-Lib seeds the first valid direction as up, and its signs are opposite to
TradingView's built-in Supertrend direction. The default signal follows the
current direction (`Buy` for up, `Sell` for down); it does not detect a flip.
Custom callbacks receive high, low, close, SuperTrend and Trend, in that order,
once both current outputs are finite.

MOM measures the difference between the current value and the value `period`
bars ago. Its default signal is `Buy` for positive momentum, `Sell` for
negative momentum, and `Stay` for zero. This reflects the current value, not a
zero-line crossing. Custom signal callbacks receive the MOM output series.

ALMA takes `period`, `sigma`, and `offset`, in that order. TA-Lib accepts a
minimum sigma of 0.01 and an offset from 0 to 1; its usual values are 6 and
0.85, respectively.

VIDYA takes `period` for the EMA smoothing factor and `cmoPeriod` for the
unsmoothed Chande Momentum Oscillator. TA-Lib requires at least 1 and 2,
respectively; its usual values are 12 and 9.

The generic `Indicators.Ma` accepts a `MovingAverageType` supported by the
installed TA-Lib version. `Default` delegates the type choice to the underlying
function, while `Disabled` bypasses smoothing. For example:

```csharp
using TANet.Contracts.Enums;
using TANet.Core;

double[] close = [10, 9, 8, 7, 6]; // newest first
var result = Indicators.Ma(close, MovingAverageType.Hma, period: 3);
```

The generic TA-Lib MA interface accepts only a period and a type. It returns one
series and uses fixed or derived settings for types with additional parameters:
T3 uses `vFactor = 0.7`, ALMA uses `sigma = 6` and `offset = 0.85`, MAMA uses
`fastLimit = 0.5` and `slowLimit = 0.05`, and VIDYA derives its CMO period as
`(3 * period + 2) / 4` using integer division. Use the standalone T3, ALMA,
MAMA, or VIDYA APIs to specify those parameters; standalone MAMA also returns
FAMA. With MAMA selected as an MA type, TA-Lib ignores the period for periods
greater than one; period one copies the input. VWMA remains a separate
indicator because it also needs a volume series.

For compatibility with existing releases, an undefined `MovingAverageType`
value currently falls back to SMA.

## Upgrading from 3.0 to 3.1

- The previously published misspelled Stochastic overloads remain available but
  are marked obsolete. Migrate to `Indicators.Stochastic`; projects that treat
  warnings as errors may need to update their calls when upgrading.
- The original `MovingAverageType` members retain their numeric values. Values
  5 through 15 now identify new members; if an application previously cast
  these undefined numbers to the enum, they no longer fall back to SMA.

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
