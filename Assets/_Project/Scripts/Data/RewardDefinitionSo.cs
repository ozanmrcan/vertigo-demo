using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "so_reward_", menuName = "WheelOfFortune/Reward Definition")]
    public class RewardDefinitionSo : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        [SerializeField]
        [Tooltip("Currency (cash/gold) scales smoothly with the zone multiplier (rounded). " +
                 "Other rewards are item counts that step up only when a whole unit is reached (floored).")]
        private bool _isCurrency;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public bool IsCurrency => _isCurrency;

        internal void InitializeForTests(bool isCurrency)
        {
            _isCurrency = isCurrency;
        }
    }
}
