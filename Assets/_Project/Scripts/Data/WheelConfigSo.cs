using UnityEngine;

namespace WheelOfFortune.Data
{
    public enum WheelTier
    {
        Bronze,
        Silver,
        Golden
    }

    [CreateAssetMenu(fileName = "so_wheel_", menuName = "WheelOfFortune/Wheel Config")]
    public class WheelConfigSo : ScriptableObject
    {
        private const int ExpectedSliceCount = 8;

        [SerializeField] private WheelTier _tier;
        [SerializeField] private Sprite _baseSprite;
        [SerializeField] private Sprite _indicatorSprite;
        [SerializeField] private string _title;
        [SerializeField] private WheelSliceData[] _slices = new WheelSliceData[ExpectedSliceCount];

        public WheelTier Tier => _tier;
        public Sprite BaseSprite => _baseSprite;
        public Sprite IndicatorSprite => _indicatorSprite;
        public string Title => _title;
        public WheelSliceData[] Slices => _slices;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slices.Length != ExpectedSliceCount)
            {
                Debug.LogWarning($"{name}: expected {ExpectedSliceCount} slices, found {_slices.Length}.", this);
            }

            var hasBomb = false;
            foreach (var slice in _slices)
            {
                if (slice != null && slice.IsBomb)
                {
                    hasBomb = true;
                    break;
                }
            }

            if (_tier == WheelTier.Bronze && !hasBomb)
            {
                Debug.LogWarning($"{name}: Bronze wheel has no bomb slice.", this);
            }

            if (_tier != WheelTier.Bronze && hasBomb)
            {
                Debug.LogWarning($"{name}: {_tier} wheel should not have a bomb slice.", this);
            }
        }
#endif
    }
}
