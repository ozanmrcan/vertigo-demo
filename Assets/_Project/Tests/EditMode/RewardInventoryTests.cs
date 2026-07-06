using NUnit.Framework;
using UnityEngine;
using WheelOfFortune.Core;
using WheelOfFortune.Data;

namespace WheelOfFortune.Tests
{
    public class RewardInventoryTests
    {
        [Test]
        public void Add_AccumulatesAmountForSameReward()
        {
            var reward = ScriptableObject.CreateInstance<RewardDefinitionSo>();
            var inventory = new RewardInventory();

            inventory.Add(reward, 100);
            inventory.Add(reward, 50);

            Assert.AreEqual(150, inventory.Amounts[reward]);
        }

        [Test]
        public void Clear_EmptiesInventory()
        {
            var reward = ScriptableObject.CreateInstance<RewardDefinitionSo>();
            var inventory = new RewardInventory();
            inventory.Add(reward, 100);

            inventory.Clear();

            Assert.AreEqual(0, inventory.Amounts.Count);
        }
    }
}
