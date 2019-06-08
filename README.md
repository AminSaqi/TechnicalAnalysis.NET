
# TechnicalAnalysis.NET
Simple use of technical analysis indicators for C#, based on TALib. 

	using TANet.Core;
	
    decimal[] input = ...;
    var result = Indicators.Sma(input, 20);
    if (result.Success)
    {
	    // use result.Ma;
	    // use result.IndicatorSignal;
    }
    else
    {
	    // show result.Message;
    }

You can pass your custom signal logic of indicators (here is the default signal logic for moving averages):

    Func<decimal[], decimal[], IndicatorSignal> customSignalLogic = (input, output) =>
         input[input.Length - 1] > output[output.Length - 1] ? IndicatorSignal.Buy :
         input[input.Length - 1] < output[output.Length - 1] ? IndicatorSignal.Sell :
         IndicatorSignal.Stay;
         
    var result = Indicators.Sma(input, 20, signalLogic: customSignalLogic);

You can also use the Candle model- it's more clean in indicators that require several inputs:

    var result = Indicators.Cci(high, low, close, 14);
    
	var candles = new Candle[] 
	{ 
		new Candle { Open = 95, High = 100, Low = 90, Close = 95, ...}, 
		...
	}
	
	var resultCandles = Indicators.Cci(candles, 14);

## Installation:

Simply via nuget package manager:

    PM> Install-Package TechnicalAnalysis.Net

Or .NET CLI:

    > dotnet add package TechnicalAnalysis.Net

## Implemented Indicators:
 - [x] Aroon
 - [x] AroonOscillator
 - [x] Atr 
 - [x] BollingerBands
 - [x] Cci
 - [x] Ema
 - [x] ExtendedMacd
 - [x] Kama
 - [x] Macd
 - [x] Mfi
 - [x] ParabolicSar
 - [x] Rsi
 - [x] Sma
 - [x] Stochastic
 - [x] Tema
 - [x] WilliamsR
 - [x] Wma

