
using TANet.Contracts.Enums;
using TicTacTec.TA.Library;

namespace TANet.Util.StaticClasses.EnumConvertors
{
    public static class MovingAverageTypes
    {
        public static MovingAverageType FromTaLib(Core.MAType maType)
        {
            switch (maType)
            {
                case Core.MAType.Ema:
                    return MovingAverageType.Ema;

                case Core.MAType.Kama:
                    return MovingAverageType.Kama;

                case Core.MAType.Sma:
                    return MovingAverageType.Sma;

                case Core.MAType.Tema:
                    return MovingAverageType.Tema;

                case Core.MAType.Wma:
                    return MovingAverageType.Wma;

                default:
                    return MovingAverageType.Sma;
            }
        }

        public static Core.MAType ToTaLib(MovingAverageType maType)
        {
            switch (maType)
            {
                case MovingAverageType.Ema:
                    return Core.MAType.Ema;

                case MovingAverageType.Kama:
                    return Core.MAType.Kama;

                case MovingAverageType.Sma:
                    return Core.MAType.Sma;

                case MovingAverageType.Tema:
                    return Core.MAType.Tema;

                case MovingAverageType.Wma:
                    return Core.MAType.Wma;

                default:
                    return Core.MAType.Sma;
            }
        }


        public static TALib.MAType ToOfficialTaLib(MovingAverageType maType)
        {
            return maType switch
            {
                MovingAverageType.Sma => TALib.MAType.SMA,
                MovingAverageType.Ema => TALib.MAType.EMA,
                MovingAverageType.Wma => TALib.MAType.WMA,
                MovingAverageType.Kama => TALib.MAType.KAMA,
                MovingAverageType.Tema => TALib.MAType.TEMA,
                _ => TALib.MAType.SMA
            };
        }
    }
}
