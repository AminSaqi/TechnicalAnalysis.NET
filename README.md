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

- Aroon and Aroon Oscillator
- ATR, Bollinger Bands and CCI
- EMA, KAMA, SMA, TEMA and WMA
- MACD and Extended MACD
- MFI, Parabolic SAR and RSI
- Stochastic (currently exposed as `Indicators.Stochsatic`)
- Williams %R

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
