
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
                MovingAverageType.Dema => TALib.MAType.DEMA,
                MovingAverageType.Trima => TALib.MAType.TRIMA,
                MovingAverageType.Mama => TALib.MAType.MAMA,
                MovingAverageType.T3 => TALib.MAType.T3,
                MovingAverageType.Hma => TALib.MAType.HMA,
                MovingAverageType.Disabled => TALib.MAType.DISABLED,
                MovingAverageType.Default => TALib.MAType.DEFAULT,
                MovingAverageType.Zlema => TALib.MAType.ZLEMA,
                MovingAverageType.Rma => TALib.MAType.RMA,
                MovingAverageType.Vidya => TALib.MAType.VIDYA,
                MovingAverageType.Alma => TALib.MAType.ALMA,
                _ => TALib.MAType.SMA
            };
        }
    }
}
