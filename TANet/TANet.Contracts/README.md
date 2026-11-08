# TechnicalAnalysis.Net.Contracts

Shared models, enums, and result types for
[TechnicalAnalysis.NET](https://github.com/AminSaqi/TechnicalAnalysis.NET).
This package targets .NET Standard 2.0 and can be used independently when an
application needs the data contracts without the indicator calculations.

```bash
dotnet add package TechnicalAnalysis.Net.Contracts --version 3.1.0
```

To calculate indicators, install `TechnicalAnalysis.Net` instead. It brings in
this package as a dependency.

## Models and results

- `TANet.Contracts.Models.Candle` holds OHLCV data, candle timestamps, trade
  count, and finalized status. Price and volume properties are `double`.
- `TANet.Contracts.Enums` contains types such as `IndicatorSignal`,
  `MovingAverageType`, and `IndicatorCalculationBase`.
- `TANet.Contracts.OperationResults.Indicators` contains the result types
  returned by indicator calculations. Results expose `Success`, `Message`,
  `IndicatorSignal`, and indicator-specific output series.

For example:

```csharp
using TANet.Contracts.Models;

var candle = new Candle
{
    Open = 12,
    High = 15,
    Low = 11,
    Close = 14,
    Volume = 1000
};
```

## Data conventions

The indicator APIs expect input arrays and candle lists in **newest-first**
order. On success, their output series have the same length and ordering as the
input, with `double.NaN` for positions without enough history.

`Success` means the calculation completed; it does not guarantee a finite
value at index 0. When `Success` is false, inspect `Message` before using
output series. The `HasValidData` and `CountValidData` extension methods are
provided by `TechnicalAnalysis.Net.Util`, not this package.

See the [project README](https://github.com/AminSaqi/TechnicalAnalysis.NET#readme)
for usage examples, supported indicators, and migration guidance.

## License

See the [project license](https://github.com/AminSaqi/TechnicalAnalysis.NET/blob/master/LICENSE).
