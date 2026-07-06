using NUnit.Framework;
using WheelOfFortune.Core;

namespace WheelOfFortune.Tests
{
    public class ZoneRulesTests
    {
        [TestCase(1, ZoneType.Normal)]
        [TestCase(5, ZoneType.Safe)]
        [TestCase(30, ZoneType.Super)]
        [TestCase(60, ZoneType.Super)]
        [TestCase(35, ZoneType.Safe)]
        public void GetZoneType_ReturnsExpectedType(int zone, ZoneType expected)
        {
            var actual = ZoneRules.GetZoneType(zone, safeInterval: 5, superInterval: 30);

            Assert.AreEqual(expected, actual);
        }
    }
}
