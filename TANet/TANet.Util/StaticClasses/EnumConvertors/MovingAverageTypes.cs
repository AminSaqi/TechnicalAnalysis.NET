
using TANet.Contracts.Enums;

namespace TANet.Util.StaticClasses.EnumConvertors
{
    public static class MovingAverageTypes
    {
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
