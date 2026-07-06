using UnityEngine;

namespace WheelOfFortune.Data
{
    [CreateAssetMenu(fileName = "so_reward_", menuName = "WheelOfFortune/Reward Definition")]
    public class RewardDefinitionSo : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
    }
}
