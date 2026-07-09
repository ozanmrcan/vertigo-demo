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

        public void Remove(RewardDefinitionSo reward, int amount)
        {
            if (!_amounts.TryGetValue(reward, out var current))
            {
                return;
            }

            var remaining = current - amount;
            if (remaining > 0)
            {
                _amounts[reward] = remaining;
            }
            else
            {
                _amounts.Remove(reward);
            }
        }

        public void Clear()
        {
            _amounts.Clear();
        }
    }
}
