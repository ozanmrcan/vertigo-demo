using System.Collections.Generic;
using WheelOfFortune.Data;

namespace WheelOfFortune.Core
{
    public class RewardInventory
    {
        private readonly Dictionary<RewardDefinitionSo, int> _amounts = new Dictionary<RewardDefinitionSo, int>();

        public IReadOnlyDictionary<RewardDefinitionSo, int> Amounts => _amounts;

        public void Add(RewardDefinitionSo reward, int amount)
        {
            _amounts.TryGetValue(reward, out var current);
            _amounts[reward] = current + amount;
        }

        public void Clear()
        {
            _amounts.Clear();
        }
    }
}
