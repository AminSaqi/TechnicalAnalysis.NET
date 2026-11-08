# TechnicalAnalysis.Net.Util

Calculation methods and output-series helpers for
[TechnicalAnalysis.NET](https://github.com/AminSaqi/TechnicalAnalysis.NET).
This package targets .NET 10, uses the official C# TA-Lib package, and depends
on `TechnicalAnalysis.Net.Contracts`.

```bash
dotnet add package TechnicalAnalysis.Net.Util --version 3.1.0
```

Most consumers should install the main `TechnicalAnalysis.Net` package and use
its `TANet.Core.Indicators` API. The main package references Util, so the
extension methods below are available there too.

## Checking a series

Add `using TANet.Util.Extensions;` to use the output-series helpers:

```csharp
using System;
using TANet.Util.Extensions;

double[] values = { 14, 13, 12, double.NaN, double.NaN };

Console.WriteLine(values.HasValidData());   // True
Console.WriteLine(values.CountValidData()); // 3

if (values.Length > 1 && double.IsFinite(values[1]))
{
    Console.WriteLine(values[1]); // Previous value: 13
}
```

`HasValidData()` checks that index 0 exists and contains a finite number. It
does not imply that older values are finite. `CountValidData()` counts all
finite values, which are not necessarily consecutive; pass it a non-null array.

## Calculation contract

`TANet.Util.StaticClasses.Indicators` provides lower-level calculation
methods used by the main API. Input and output series use **newest-first**
ordering. Multi-series inputs must have equal lengths and values at the same
index must describe the same observation.

Successful output series have the input length. Values without enough history
are `double.NaN`. Some calculations can also return a non-finite value for
other reasons, such as a VWMA window with no volume. Check `Success` before
accessing the output, and check the relevant series before using its values.
Custom signal callbacks are not called when the current output required by the
signal is non-finite.

See the [project README](https://github.com/AminSaqi/TechnicalAnalysis.NET#readme)
for the full contract, supported indicators, and usage examples.

## License

See the [project license](https://github.com/AminSaqi/TechnicalAnalysis.NET/blob/master/LICENSE).
