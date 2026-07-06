namespace WheelOfFortune.Core
{
    public enum ZoneType
    {
        Normal,
        Safe,
        Super
    }

    public static class ZoneRules
    {
        public static ZoneType GetZoneType(int zone, int safeInterval, int superInterval)
        {
            if (superInterval > 0 && zone % superInterval == 0)
            {
                return ZoneType.Super;
            }

            if (safeInterval > 0 && zone % safeInterval == 0)
            {
                return ZoneType.Safe;
            }

            return ZoneType.Normal;
        }
    }
}
