using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "so_game_config", menuName = "WheelOfFortune/Game Config")]
    public class GameConfigSo : ScriptableObject
    {
        [SerializeField] private int _safeZoneInterval = 5;
        [SerializeField] private int _superZoneInterval = 30;
        [SerializeField] private WheelConfigSo _bronzeWheel;
        [SerializeField] private WheelConfigSo _silverWheel;
        [SerializeField] private WheelConfigSo _goldenWheel;
        [SerializeField] private AnimationCurve _rewardMultiplierByZone = AnimationCurve.Linear(1, 1, 30, 3);

        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;
        public WheelConfigSo BronzeWheel => _bronzeWheel;
        public WheelConfigSo SilverWheel => _silverWheel;
        public WheelConfigSo GoldenWheel => _goldenWheel;
        public AnimationCurve RewardMultiplierByZone => _rewardMultiplierByZone;

        internal void InitializeForTests(
            int safeZoneInterval,
            int superZoneInterval,
            WheelConfigSo bronzeWheel,
            WheelConfigSo silverWheel,
            WheelConfigSo goldenWheel,
            AnimationCurve rewardMultiplierByZone)
        {
            _safeZoneInterval = safeZoneInterval;
            _superZoneInterval = superZoneInterval;
            _bronzeWheel = bronzeWheel;
            _silverWheel = silverWheel;
            _goldenWheel = goldenWheel;
            _rewardMultiplierByZone = rewardMultiplierByZone;
        }
    }
}
